using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using BlessTheWeb.Core;
using BlessTheWeb.Core.Extensions;
using BlessTheWeb.Models;
using JustConfessing;
using Raven.Client;

namespace BlessTheWeb.Controllers
{
    [HandleError]
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            if (TempData["absolutionId"] != null)
            {
                ViewData["ShowBlessing"] = true;
            }

            return View(DefaultHomeViewModel());
        }

        private HomeViewModel DefaultHomeViewModel()
        {
            HomeViewModel viewModel = new HomeViewModel();
            viewModel.Sins = _indulgeMeService.GetFiveRandomSins();
            viewModel.TopRedeemed = _indulgeMeService.GetFiveRandomBlessedSins();
            viewModel.BlessedIndulgences = _indulgeMeService.GetFiveLatestIndulgences();
            viewModel.SiteInfo = GetSiteInfo(HttpContext, MvcApplication.CurrentSession);
            return viewModel;
        }

        private SiteSummaryInfo GetSiteInfo(HttpContextBase httpContext, IDocumentSession ravenDbSession)
        {
            var siteInfo = httpContext.Cache["siteInfo"] as SiteSummaryInfo;
            if (siteInfo == null)
            {
                siteInfo = _indulgeMeService.GetSiteSummaryInfo();
                httpContext.Cache.Add("siteInfo", siteInfo, null, Cache.NoAbsoluteExpiration,
                                      TimeSpan.FromHours(1), CacheItemPriority.Normal, null);
            }
            return siteInfo;
        }

        [HttpPost]
        public ActionResult Index(string confession)
        {
            // store the confession as a sin
            var sin = new Sin() { Content = confession, Source = "JC" };
            MvcApplication.CurrentSession.Store(sin);

            Indulgence con = new Indulgence()
            {
                Confession = confession.Truncate(150),
                DateConfessed = DateTime.Now,
                IsBlessed = false,
                IsConfession = true,
                SinId = sin.Id,
                Tweeted=false,
                Name="Anonymous"
            };
            MvcApplication.CurrentSession.Store(con);

            MvcApplication.CurrentSession.SaveChanges();

            return RedirectToAction("Confess", new {id=con.Id.IdValue()});
        }

        public ActionResult Confess(string id)
        {
            var con = MvcApplication.CurrentSession.Load<Indulgence>("indulgences/" + id);
            ViewData["AbsolutionMessage"] = "Who will you repent for?";
            ViewData["cid"] = con.Id.IdValue();
            return View(con);
        }

        [HttpPost]
        public ActionResult SelectCharity(int? absolutionid, int? charityId, string charityName, string name, string email)
        {
            if (!absolutionid.HasValue || !charityId.HasValue) return RedirectToAction("Index");

            var con = MvcApplication.CurrentSession.Load<Indulgence>("indulgences/" + absolutionid);
            con.CharityId = charityId.Value;
            con.CharityName = charityName.Truncate(80);
            if (!string.IsNullOrWhiteSpace(name)) con.Name = name.Truncate(60);
            if (!string.IsNullOrWhiteSpace(email)) con.DonorEmailAddress = email.Truncate(200);
            MvcApplication.CurrentSession.Store(con);
            MvcApplication.CurrentSession.SaveChanges();
            return EnterSimpleDonationProcess(absolutionid.Value, charityId.Value);
        }

        private ActionResult EnterSimpleDonationProcess(int confessionId, int charityId)
        {
            string returnUrl = string.Format(ConfigurationManager.AppSettings["JGSDIReturnUrlFormat"], System.Web.HttpContext.Current.Request.Url.Authority, confessionId);
            return Redirect(
                string.Format(ConfigurationManager.AppSettings["JGSDIUrlFormat"], charityId, returnUrl));
        }

        public ActionResult Absolve(int id)
        {
            var sin = MvcApplication.CurrentSession.Load<Sin>("sins/"+id.ToString());
            var con = new Indulgence()
            {
                Confession = sin.Content.Truncate(250),
                DateConfessed = DateTime.Now,
                IsBlessed = false,
                IsConfession = false,
                SinId = sin.Id
            };
            MvcApplication.CurrentSession.Store(con);
            MvcApplication.CurrentSession.SaveChanges();
            ViewData["cid"] = con.Id.IdValue();
            return View("Absolve", con);
        }

        public ActionResult Bless(string id)
        {
            var con = MvcApplication.CurrentSession.Load<Indulgence>("indulgences/" + id);
            con.IsBlessed = true;
            MvcApplication.CurrentSession.Store(con);
            MvcApplication.CurrentSession.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult Absolved(string id)
        {
            ViewData["ShowBlessing"] = true;

            return View("Index", DefaultHomeViewModel());
        }

        public ActionResult RebuildIndexes()
        {
            new RavenIndexManager().SetUpIndexes(MvcApplication.CurrentSession);
            return RedirectToAction("Index", "Home");
        }
    }
}
