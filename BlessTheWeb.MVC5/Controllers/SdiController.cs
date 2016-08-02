using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JustGiving.Api.Sdk;
using BlessTheWeb.Core.Repository;
using BlessTheWeb.Core;
using JustGiving.Api.Sdk.Model.Donation;

namespace BlessTheWeb.MVC5.Controllers
{
    public class SdiController : Controller
    {
        private readonly IDatabase _db;
        private readonly IndulgeMeService _indulgeMeService;

        public SdiController(IDatabase db, IndulgeMeService indulgeMeService)
        {
            _db = db;
            _indulgeMeService = indulgeMeService;
        }

        // GET: Sdi
        public ActionResult Return(string guid, int? donationId)
        {
            if (!donationId.HasValue)
                return RedirectToAction("Index", "Home");

            var config = new ClientConfiguration(
                ConfigurationManager.AppSettings["JgApiBaseUrl"],
                ConfigurationManager.AppSettings["JGApiKey"],
                1);

            var client = new JustGivingClient(config);

            DonationStatus donationStatus = null;
            try
            {
                donationStatus = client.Donation.RetrieveStatus(donationId.Value);
            }
            catch
            {

            }

            if ((donationStatus != null && donationStatus.Status == "Accepted")
                || ConfigurationManager.AppSettings["SkipDonationReferenceCheck"] == "true")
            {
                var indulgence = _db.GetIndulgenceByGuid(guid);
                if (donationStatus != null)
                {
                    _db.Absolve(guid, donationStatus.DonationId, donationStatus.DonationRef, donationStatus.Amount,
                        donationStatus.Reference);
                } else if (ConfigurationManager.AppSettings["SkipDonationReferenceCheck"] == "true")
                {
                    _db.Absolve(guid, 1, "1", 10, "not-a-real-donation");
                }

                _indulgeMeService.GenerateIndulgence(indulgence, 
                    System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "fonts"),
                    System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Content"));

                TempData["absolutionId"] = guid;

                ControllerContext.RequestContext.HttpContext.Cache.Remove("siteInfo");
                ControllerContext.RequestContext.HttpContext.Response.RemoveOutputCacheItem(Url.Action("GetLatest", "Api"));

                return RedirectToAction("Index", "Indulgence", new { guid=guid });
            }

            return HttpNotFound();
        }
    }
}