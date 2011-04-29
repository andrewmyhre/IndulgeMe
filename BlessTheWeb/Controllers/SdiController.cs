using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;
using BlessTheWeb.Code;
using BlessTheWeb.Core;
using BlessTheWeb.Core.Extensions;
using JustConfessing;
using JustGiving.Api.Sdk;
using JustGiving.Api.Sdk.Model.Donation;
using log4net;

namespace BlessTheWeb.Controllers
{
    public class SdiController : ControllerBase
    {
        private IIndulgenceEmailer indulgenceEmailer;
        //
        // GET: /Sdi/
        private ILog log = LogManager.GetLogger(typeof (SdiController));
        private ITweetOutbox _tweetOutbox = null;
        public SdiController()
        {
            _tweetOutbox = new TweetOutbox(ConfigurationManager.AppSettings["TweetOutboxDirectory"]);
            indulgenceEmailer = new EmailProcessIndulgenceEmailer();
        }
        public SdiController(ITweetOutbox tweetOutbox)
        {
            _tweetOutbox = tweetOutbox;
        }

        Random r = new Random();
        public ActionResult Absolve(int id, int? donationId)
        {
            var config = new ClientConfiguration(
                "https://api.staging.justgiving.com/",
                ConfigurationManager.AppSettings["JGApiKey"],
                1);
            var client = new JustGivingClient(config);

            DonationStatus donation = null;
            if (donationId.HasValue)
                donation = client.Donation.RetrieveStatus(donationId.Value);
            else
            {
                donationId = 1;
                donation = new DonationStatus()
                               {Amount = new decimal(r.NextDouble()*100), DonationId = 1, Reference = "1"};
            }

            var indulgence = MvcApplication.CurrentSession.Load<Indulgence>(id.ToRavenDbId("indulgences"));
            if (indulgence == null)
                return new HttpNotFoundResult();

            indulgence.IsBlessed = true;
            if (donation != null)
            {
                indulgence.AmountDonated = donation.Amount;
                indulgence.JustGivingDonationId = donationId.Value;
                indulgence.DonationReference = donation.Reference;
            }

            var sin = MvcApplication.CurrentSession.Load<Sin>(indulgence.SinId);
            sin.TotalDonationCount++;
            if (donation != null) sin.TotalDonated += donation.Amount;

            MvcApplication.CurrentSession.SaveChanges();

            MvcApplication.TotalDonated = MvcApplication.CurrentSession
                .Query<Indulgence>("BlessedIndulgences").ToList().Sum(a => a.AmountDonated);

            string storagePath = HostingEnvironment.MapPath("~/content/indulgences");
            string pdfFilename = string.Format("{0}/indulgence.pdf", indulgence.Id.IdValue());
            string imageFilename = string.Format("{0}/indulgence.png", indulgence.Id.IdValue());
            
            pdfFilename = Path.Combine(storagePath, pdfFilename);
            imageFilename = Path.Combine(storagePath, imageFilename);

            _indulgeMeService.GenerateIndulgence(pdfFilename, imageFilename, indulgence, 
                HostingEnvironment.MapPath("~/content"), HostingEnvironment.MapPath("~/content"));

            // if the user supplied an email address send the pdf to them
            if (!string.IsNullOrWhiteSpace(indulgence.DonorEmailAddress))
            {
                string donorEmailAddress = indulgence.DonorEmailAddress;
                string donorName = string.IsNullOrWhiteSpace(indulgence.Name) ? "" : indulgence.Name;
                indulgenceEmailer.Send(indulgence, pdfFilename);
            }

            try
            {
                // add the indulgence to the queue to tweet
                _tweetOutbox.Add(indulgence);
            }
            catch (Exception e)
            {
                log.Error("Could not tweet!", e);
            }

            TempData["absolutionId"] = id;

            // invalidate cache
            ControllerContext.RequestContext.HttpContext.Cache.Remove("siteInfo");
            ControllerContext.RequestContext.HttpContext.Response.RemoveOutputCacheItem(Url.Action("GetLatest", "Api"));

            return RedirectToAction("Index", "Indulgence", new {id=indulgence.Id.IdValue()});
        }

        public void EmailTest()
        {
            var indulgence = new Indulgence()
                                 {
                                     Name="andrew myhre",
                                     CharityName="test charity",
                                     DonorEmailAddress="andrew.myhre@gmail.com"
                                 };
            
            indulgenceEmailer.Send(indulgence, @"C:\workspace\git\BlessTheWeb\BlessTheWeb\Content\indulgences\6145.pdf");
        }

        [Obsolete("use a IIndulgenceEmaier instead",true)]
        private static void SendIndulgenceEmail(Indulgence indulgence, string indulgenceFilePath)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(ConfigurationManager.AppSettings["emailFromAddress"], "IndulgeMe.cc");
            message.To.Add(new MailAddress(indulgence.DonorEmailAddress, indulgence.Name));

            var templateDoc = XDocument.Load(HostingEnvironment.MapPath("~/content/emailTemplates/indulgenceEmail.xml"));
            string subject = templateDoc.Element("email").Element("subject").Value;
            string textBody = templateDoc.Element("email").Element("body").Element("text").Value;
            string htmlBody = templateDoc.Element("email").Element("body").Element("html").Value;
            subject = subject
                        .Replace("@DonorName", indulgence.Name)
                        .Replace("@CharityName", indulgence.CharityName);
            textBody = textBody
                        .Replace("@DonorName", indulgence.Name)
                        .Replace("@CharityName", indulgence.CharityName);
            htmlBody = htmlBody
                        .Replace("@DonorName", indulgence.Name)
                        .Replace("@CharityName", indulgence.CharityName);

            message.Subject = subject;
            message.Body = textBody;
            message.Attachments.Add(new Attachment(indulgenceFilePath));
            SmtpClient smtp = new SmtpClient();
            smtp.Port = int.Parse(ConfigurationManager.AppSettings["smtpPort"]);
            smtp.Host = ConfigurationManager.AppSettings["smtpServer"];
            smtp.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["smtpUsername"],
                                                                ConfigurationManager.AppSettings["smtpPassword"]);
            smtp.EnableSsl = true;
            smtp.Send(message);
        }
    }
}
