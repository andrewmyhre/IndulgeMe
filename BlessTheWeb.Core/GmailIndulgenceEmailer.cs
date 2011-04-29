using System;
using System.Configuration;
using System.Net.Mail;
using System.Web.Hosting;
using System.Xml.Linq;

namespace BlessTheWeb.Core
{
    [Obsolete("Use EmailProcessing", true)]
    public class GmailIndulgenceEmailer : IIndulgenceEmailer
    {
        public void Send(Indulgence indulgence, string indulgenceFilePath)
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