using System;
using System.Configuration;
using System.Web.Hosting;
using System.Xml.Linq;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using System.Threading.Tasks;

namespace BlessTheWeb.Core
{
    public class AmazonSesIndulgenceEmailer : IIndulgenceEmailer
    {
        private Amazon.SimpleEmail.AmazonSimpleEmailServiceClient service;
        public async Task Send(Indulgence indulgence, string indulgenceFilePath)
        {
            service =
                new AmazonSimpleEmailServiceClient(
                    ConfigurationManager.AppSettings["awsAccessKeyId"],
                    ConfigurationManager.AppSettings["awsAccessSecret"]);

            SendEmailRequest request = new SendEmailRequest();
            request.Destination = BuildDestination();
            request.Message=BuildMessage(indulgence);
            request.Source="andrew.myhre@gmail.com";

            var response = service.SendEmail(request);
        }

        private Message BuildMessage(Indulgence indulgence)
        {
            var templateDoc = XDocument.Load(HostingEnvironment.MapPath("~/content/emailTemplates/indulgenceEmail.xml"));
            string subjectText = templateDoc.Element("email").Element("subject").Value;
            string textBody = templateDoc.Element("email").Element("body").Element("text").Value;
            string htmlBody = templateDoc.Element("email").Element("body").Element("html").Value;
            subjectText = subjectText
                .Replace("@DonorName", indulgence.Name)
                .Replace("@CharityName", indulgence.CharityName);
            textBody = textBody
                .Replace("@DonorName", indulgence.Name)
                .Replace("@CharityName", indulgence.CharityName);
            htmlBody = htmlBody
                .Replace("@DonorName", indulgence.Name)
                .Replace("@CharityName", indulgence.CharityName);

            Content subject = new Content();
            subject.Charset = "utf-8";
            subject.Data = subjectText;

            Content html = new Content();
            html.Charset="UTF-8";
            html.Data= htmlBody;

            Content text = new Content();
            text.Charset="UTF-8";
            text.Data=textBody;

            Body body = new Body();
            body.Html=html;
            body.Text=text;

            Message message = new Message();
            message.Body=body;
            message.Subject=subject;

            return message;
        }

        private Destination BuildDestination()
        {
            Destination destination = new Destination();
            destination.ToAddresses.Add("andrew.myhre@gmail.com");
            return destination;
        }
    }
}