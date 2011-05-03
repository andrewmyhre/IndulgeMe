using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using BlessTheWeb.Code;
using BlessTheWeb.Core;
using BlessTheWeb.Core.Extensions;
using BlessTheWeb.Models;
using JustConfessing;
using Raven.Client.Linq;

namespace BlessTheWeb.Controllers
{
    public class IndulgenceController : ControllerBase
    {
        const int pageSize = 12;
        private string contentPath = HostingEnvironment.MapPath("~/content");
        //
        // GET: /Confessions/
        public ActionResult Index(int id)
        {
            if (TempData["absolutionId"] != null)
            {
                ViewData["ShowBlessing"] = true;
            }

            var absolutionViewModel = new IndulgencesViewModel();
            absolutionViewModel.Indulgence = _indulgeMeService.GetIndulgence(id.ToRavenDbId("Indulgences"));
            if (absolutionViewModel.Indulgence == null)
                return new HttpNotFoundResult();

            if (absolutionViewModel.Indulgence.SinId == null)
                return new HttpNotFoundResult();

            absolutionViewModel.Sin = _indulgeMeService.GetSin(absolutionViewModel.Indulgence.SinId);
            absolutionViewModel.Sin.AllAbsolutions = _indulgeMeService.AllIndulgencesForSin(absolutionViewModel.Indulgence.SinId);

            absolutionViewModel.TotalDonationCount = absolutionViewModel.Sin.AllAbsolutions.Count();
            absolutionViewModel.TotalDonated = absolutionViewModel.Sin.AllAbsolutions.Sum(a => a.AmountDonated);
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

        public ActionResult GenerateImage(int id)
        {
            string storagePath = System.Web.Hosting.HostingEnvironment.MapPath("/content/indulgences");
            var indulgence = _indulgeMeService.GetIndulgence(id.ToRavenDbId("Indulgences"));
            if (indulgence == null)
            {
                return new HttpNotFoundResult();
            }

            string fileName = string.Format("{0}/indulgence.pdf", indulgence.Id.IdValue());
            string thumbnailFileName = string.Format("{0}/indulgence.png", indulgence.Id.IdValue());
            fileName = Path.Combine(storagePath, fileName);
            thumbnailFileName = Path.Combine(storagePath, thumbnailFileName);
            _indulgeMeService.GenerateIndulgence(fileName, thumbnailFileName, indulgence,
                contentPath, contentPath);
            
            return new ContentResult(){Content = "ok", ContentType="text/html"};
        }

        public ActionResult GenerateAll()
        {
            StringBuilder sb = new StringBuilder();
            var indulgences = _indulgeMeService.AllIndulgences(0,9999);
            string storagePath = System.Web.Hosting.HostingEnvironment.MapPath("/content/indulgences");
            

            foreach (var indulgence in indulgences)
            {
                string pdfPath = Path.Combine(storagePath, string.Format("{0}/indulgence.pdf", indulgence.Id.IdValue()));
                string imagePath = Path.Combine(storagePath, string.Format("{0}/indulgence.png", indulgence.Id.IdValue()));
                _indulgeMeService.GenerateIndulgence(pdfPath, imagePath, indulgence,
                    contentPath, contentPath);
                sb.AppendFormat("<p>{0}</p>", indulgence.Id);
            }

            return new ContentResult() {Content = sb.ToString(), ContentType = "text/html"};
        }

        public ActionResult List(int? page)
        {
            
            page = page.HasValue ? page.Value : 1;
            var viewModel = new AbsolutionsViewModel();
            int totalIndulgences = _indulgeMeService.IndulgencesCount();
            viewModel.Indulgences = _indulgeMeService.AllIndulgences(page.Value-1, pageSize);
            viewModel.SiteInfo = _indulgeMeService.GetSiteSummaryInfo();
            viewModel.Page = page.Value;
            viewModel.NextPage = page.Value + 1;
            viewModel.PreviousPage = page.Value > 1 ? page.Value - 1 : 0;
            viewModel.CurrentPage = page.Value;
            viewModel.ShowNextPageLink = (totalIndulgences / pageSize)+1 > page.Value;
            viewModel.ShowPreviousPageLink = page.Value > 1;

            viewModel.PagingStart = viewModel.CurrentPage - 5 > 1 ? viewModel.CurrentPage - 5 : 1;
            viewModel.PagingEnd = viewModel.CurrentPage + 5 < (totalIndulgences / pageSize) + 1 ? viewModel.CurrentPage + 5 : (totalIndulgences / pageSize) + 1;

            return View(viewModel);
        }
    }
}

namespace BlessTheWeb.Models
{
    public class AbsolutionsViewModel
    {
        public IEnumerable<Indulgence> Indulgences { get; set; }
        public SiteSummaryInfo SiteInfo { get; set; }
        public int Page { get; set; }
        public int NextPage { get; set; }
        public int PreviousPage { get; set; }
        public bool ShowNextPageLink { get; set; }
        public bool ShowPreviousPageLink { get; set; }

        public int CurrentPage { get; set; }
        public int PagingStart { get; set; }
        public int PagingEnd { get; set; }
    }
}
