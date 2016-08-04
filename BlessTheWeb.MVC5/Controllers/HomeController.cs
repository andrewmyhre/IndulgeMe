﻿using BlessTheWeb.Core;
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

        public HomeController(IIndulgeMeService indulgeMeService)
        {
            _indulgeMeService = indulgeMeService;
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
            var sin = new Sin() { Content = confession, Source = "JC", Guid=Guid.NewGuid() };
                
            indulgence = new Indulgence()
            {
                Confession = confession.Truncate(150),
                DateConfessed = DateTime.Now,
                IsBlessed = false,
                IsConfession = true,
                Sin = sin,
                Tweeted = false,
                Name = "Anonymous",
                Guid=Guid.NewGuid(),
                BackgroundImageName = "parchment3"
            };
            _indulgeMeService.SaveIndulgence(indulgence);

            return RedirectToAction("Confess", new { guid = indulgence.Guid });
        }

        public ActionResult Confess(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid))
                return RedirectToAction("Index");

            var indulgence = _indulgeMeService.GetIndulgenceByGuid(guid);
            ViewData["cid"] = guid;

            return View(indulgence);
        }

        [HttpPost]
        public ActionResult EnterDonationProcess(string guid, int? charityId, string charityName, string name, string email)
        {
            if (string.IsNullOrWhiteSpace(guid) || !charityId.HasValue) return RedirectToAction("Index");

            _indulgeMeService.SetCharityDetails(guid, charityId.Value, charityName, name, email);
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

            var indulgence = _indulgeMeService.CreateIndulgenceForSin(guid);
            ViewData["cid"] = indulgence.Guid;
            return View("Absolve", indulgence);
        }

        public ActionResult Absolved(string id)
        {
            ViewData["ShowBlessing"] = true;

            return View("Index", DefaultHomeViewModel());
        }
    }
}