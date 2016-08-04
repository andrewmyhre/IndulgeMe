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
using log4net;

namespace BlessTheWeb.MVC5.Controllers
{
    public class SdiController : Controller
    {
        private readonly IIndulgeMeService _indulgeMeService;
        private readonly ILog _log;

        public SdiController(IIndulgeMeService indulgeMeService, ILog log)
        {
            _indulgeMeService = indulgeMeService;
            _log = log;
        }

        // GET: Sdi
        public ActionResult Return(string guid, int? donationId)
        {
            try
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
                    if (donationStatus != null)
                    {
                        _indulgeMeService.Absolve(guid, donationStatus.DonationId, donationStatus.DonationRef,
                            donationStatus.Amount,
                            donationStatus.Reference);
                    }
                    else if (ConfigurationManager.AppSettings["SkipDonationReferenceCheck"] == "true")
                    {
                        _indulgeMeService.Absolve(guid, 1, "1", 10, "not-a-real-donation");
                    }

                    var indulgence = _indulgeMeService.GetIndulgenceByGuid(guid);
                    _indulgeMeService.GenerateIndulgence(indulgence,
                        System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "fonts"),
                        System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Content"));

                    TempData["absolutionId"] = guid;

                    ControllerContext.RequestContext.HttpContext.Cache.Remove("siteInfo");
                    ControllerContext.RequestContext.HttpContext.Response.RemoveOutputCacheItem(Url.Action("GetLatest",
                        "Api"));

                    return RedirectToAction("Index", "Indulgence", new {guid = guid});
                }

                return HttpNotFound();
            }
            catch (Exception ex)
            {
                _log.Error("unhandled error returning from SDI", ex);
                return new EmptyResult();
            }
        }

        [HttpGet]
        public ActionResult FakeSdi(string guid, int charityId, decimal amount, string exitUrl, string currency,string reference)
        {
            return View(new FakeSdiViewModel
            {
                CharityId = charityId,
                Amount = amount,
                ExitUrl = exitUrl,
                Currency=currency,
                Reference = reference
            });
        }

        private static Random r = new Random();
        [HttpPost]
        public ActionResult FakeSdi(int charityId, decimal amount, string exitUrl, string currency, string reference)
        {
            return Redirect(exitUrl.Replace("JUSTGIVING-DONATION-ID", r.Next(1000000).ToString()));
        }

    }
    public class FakeSdiViewModel
    {
        public int CharityId { get; set; }
        public decimal Amount { get; set; }
        public string ExitUrl { get; set; }
        public string Currency { get; set; }
        public string Reference { get; set; }
    }
}