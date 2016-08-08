using BlessTheWeb.Core;
using BlessTheWeb.Core.Repository;
using BlessTheWeb.MVC5.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace BlessTheWeb.MVC5.Controllers
{
    public class IndulgenceController : BlessTheWebController
    {
        private readonly IIndulgeMeService _indulgeMeService;
        private readonly IIndulgenceEmailer _indulgenceEmailer;
        private readonly ILog _log;
        const int pageSize = 12;
        private string contentPath = HostingEnvironment.MapPath("~/content");

        public IndulgenceController(IIndulgeMeService indulgeMeService, IIndulgenceEmailer indulgenceEmailer, ILog log)
        {
            _indulgeMeService = indulgeMeService;
            _indulgenceEmailer = indulgenceEmailer;
            _log = log;
        }

        //
        // GET: /Indulgence/
        public ActionResult Index(string guid)
        {
            if (TempData["absolved"] != null || !string.IsNullOrWhiteSpace(Request.QueryString["congratulations"]))
            {
                ViewData["ShowBlessing"] = true;
            }

            var absolutionViewModel = new IndulgencesViewModel();
            absolutionViewModel.Indulgence = _indulgeMeService.GetIndulgenceByGuid(guid);
            if (absolutionViewModel.Indulgence == null)
                return HttpNotFound("Couldn't find that indulgence");

            if (absolutionViewModel.Indulgence.Sin == null)
                return HttpNotFound("Couldn't find that sin");

            absolutionViewModel.Sin = absolutionViewModel.Indulgence.Sin;
            absolutionViewModel.Sin.Indulgences = _indulgeMeService.AllIndulgencesForSin(absolutionViewModel.Indulgence.Sin).ToList();

            absolutionViewModel.TotalDonationCount = absolutionViewModel.Sin.Indulgences.Count();
            absolutionViewModel.TotalDonated = absolutionViewModel.Sin.Indulgences.Sum(a => a.AmountDonated);
            absolutionViewModel.ImageAlt =
                string.Format("{0} {1} donated {2:c} to {3} on the {4} of {5:MMMM}, {5:yyyy}",
                absolutionViewModel.Indulgence.Confession,
                absolutionViewModel.Indulgence.Name,
                absolutionViewModel.Indulgence.AmountDonated,
                absolutionViewModel.Indulgence.CharityName,
                absolutionViewModel.Indulgence.DateConfessed,
                TextUtils.DayOfMonth(absolutionViewModel.Indulgence.DateConfessed),
                absolutionViewModel.Indulgence);

            return View(absolutionViewModel);
        }

        public ActionResult Email(string guid)
        {
            var indulgence = _indulgeMeService.GetIndulgenceByGuid(guid);

            try
            {
                _indulgenceEmailer.Send(indulgence, string.Format("{0}/indulgence/pdf/{1}",
                    ConfigurationManager.AppSettings["WebsiteHostName"], indulgence.Guid));
            }
            catch (Exception ex)
            {
                _log.Error("Failed to send email", ex);
                throw;
            }

            return new ContentResult()
            {
                Content = "sent",
                ContentType = "text/plain",
                ContentEncoding = Encoding.UTF8
            };
        }

        public ActionResult GenerateImage(string guid)
        {
            var indulgence = _indulgeMeService.GetIndulgenceByGuid(guid);
            if (indulgence == null)
            {
                return new HttpNotFoundResult();
            }

            string fileName = string.Format("{0}/indulgence.pdf", indulgence.Guid);
            string thumbnailFileName = string.Format("{0}/indulgence.png", indulgence.Guid);
            _indulgeMeService.GenerateIndulgence(indulgence, contentPath, contentPath);

            return new ContentResult() { Content = "ok", ContentType = "text/html" };
        }

        public ActionResult Image(string guid, int size)
        {
            var indulgence = _indulgeMeService.GetIndulgenceByGuid(guid);
            if (indulgence == null)
                return HttpNotFound();
            return new FileContentResult(_indulgeMeService.GetIndulgenceImage(indulgence, size), "img/png");
        }

        public ActionResult Pdf(string guid)
        {
            var indulgence = _indulgeMeService.GetIndulgenceByGuid(guid);
            if (indulgence == null)
                return HttpNotFound();
            return new FileContentResult(_indulgeMeService.GetIndulgencePdf(indulgence), "application/pdf");
        }

        public ActionResult GenerateAll()
        {
            StringBuilder sb = new StringBuilder();
            var indulgences = _indulgeMeService.AllIndulgences(0, 9999);

            foreach (var indulgence in indulgences)
            {
                _indulgeMeService.GenerateIndulgence(indulgence, contentPath, contentPath);
                sb.AppendFormat("<p>{0}</p>", indulgence.Guid);
            }

            return new ContentResult() { Content = sb.ToString(), ContentType = "text/html" };
        }

        public ActionResult List(int? page)
        {

            page = page.HasValue ? page.Value : 1;
            var viewModel = new AbsolutionsViewModel();
            int totalIndulgences = _indulgeMeService.IndulgencesCount();
            viewModel.Indulgences = _indulgeMeService.AllIndulgences(page.Value - 1, pageSize);
            viewModel.SiteInfo = _indulgeMeService.GetSiteSummaryInfo();
            viewModel.Page = page.Value;
            viewModel.NextPage = page.Value + 1;
            viewModel.PreviousPage = page.Value > 1 ? page.Value - 1 : 0;
            viewModel.CurrentPage = page.Value;
            viewModel.ShowNextPageLink = (totalIndulgences / pageSize) + 1 > page.Value;
            viewModel.ShowPreviousPageLink = page.Value > 1;

            viewModel.PagingStart = viewModel.CurrentPage - 5 > 1 ? viewModel.CurrentPage - 5 : 1;
            viewModel.PagingEnd = viewModel.CurrentPage + 5 < (totalIndulgences / pageSize) + 1 ? viewModel.CurrentPage + 5 : (totalIndulgences / pageSize) + 1;

            return View(viewModel);
        }
    }
}