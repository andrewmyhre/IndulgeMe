using BlessTheWeb.Core;
using BlessTheWeb.Core.Repository;
using BlessTheWeb.Core.Extensions;
using BlessTheWeb.MVC5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Configuration;

namespace BlessTheWeb.MVC5.Controllers
{
    [RoutePrefix("")]
    public class HomeController : Controller
    {
        private readonly IIndulgeMeService _indulgeMeService;
        private readonly IDatabase _db;

        public HomeController(IIndulgeMeService indulgeMeService, IDatabase db)
        {
            _indulgeMeService = indulgeMeService;
            _db = db;
        }

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
            viewModel.SiteInfo = GetSiteInfo(HttpContext);
            return viewModel;
        }

        private SiteSummaryInfo GetSiteInfo(HttpContextBase httpContext)
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
        public ActionResult BeginConfession(string confession)
        {
            // store the confession as a sin
            Indulgence indulgence = null;
            using (var session = _db.OpenSession())
            {
                var sin = new Sin() { Content = confession, Source = "JC", Guid=Guid.NewGuid() };
                session.Store(sin);
                indulgence = new Indulgence()
                {
                    Confession = confession.Truncate(150),
                    DateConfessed = DateTime.Now,
                    IsBlessed = false,
                    IsConfession = true,
                    SinGuid = sin.Guid,
                    Tweeted = false,
                    Name = "Anonymous",
                    Guid=Guid.NewGuid(),
                    BackgroundImageName = "parchment3"
                };
                
                session.Store(indulgence);
                session.SaveChanges();
            }

            return RedirectToAction("Confess", new { guid = indulgence.Guid });
        }

        public ActionResult Confess(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid))
                return RedirectToAction("Index");

            var indulgence = _db.GetIndulgenceByGuid(guid);
            ViewData["cid"] = guid;

            return View(indulgence);
        }

        [HttpPost]
        public ActionResult EnterDonationProcess(string guid, int? charityId, string charityName, string name, string email)
        {
            if (string.IsNullOrWhiteSpace(guid) || !charityId.HasValue) return RedirectToAction("Index");

            using (var session = _db.OpenSession())
            {
                var indulgence = _db.GetIndulgenceByGuid(guid);
                indulgence.CharityId = charityId.Value;
                indulgence.CharityName = charityName.Truncate(80);
                if (!string.IsNullOrWhiteSpace(name)) indulgence.Name = name.Truncate(60);
                if (!string.IsNullOrWhiteSpace(email)) indulgence.DonorEmailAddress = email.Truncate(200);
                session.Store(indulgence);
                session.SaveChanges();
            }
            return EnterSimpleDonationProcess(guid, charityId.Value);
        }

        private ActionResult EnterSimpleDonationProcess(string guid, int charityId)
        {
            string returnUrl = string.Format(ConfigurationManager.AppSettings["JGSDIReturnUrlFormat"], System.Web.HttpContext.Current.Request.Url.Authority, guid);
            return Redirect(
                string.Format(ConfigurationManager.AppSettings["JGSDIUrlFormat"],
                charityId, 
                Url.Encode(returnUrl), 
                guid));
        }

        public ActionResult Absolve(string guid)
        {
            var sin = _db.GetSinByGuid(guid);

            using (var session = _db.OpenSession())
            {
                var con = new Indulgence()
                {
                    Confession = sin.Content.Truncate(250),
                    DateConfessed = DateTime.Now,
                    IsBlessed = false,
                    IsConfession = false,
                    SinGuid = sin.Guid
                };
                session.Store(con);
                session.SaveChanges();
                ViewData["cid"] = con.Id;
                return View("Absolve", con);
            }
            
        }

        public ActionResult Bless(string id)
        {
            using (var session = _db.OpenSession())
            {
                var indulgence = _db.GetIndulgence(id);
                indulgence.IsBlessed = true;
                session.Store(indulgence);
                session.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        public ActionResult Absolved(string id)
        {
            ViewData["ShowBlessing"] = true;

            return View("Index", DefaultHomeViewModel());
        }
    }
}