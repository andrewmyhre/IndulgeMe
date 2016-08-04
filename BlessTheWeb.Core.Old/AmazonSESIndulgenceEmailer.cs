using System;
using System.Configuration;
using System.Web.Hosting;
using System.Xml.Linq;
using Amazon.SimpleEmail.Model;

namespace BlessTheWeb.Core
{
    [Obsolete("Use EmailProcessing", true)]
    public class AmazonSesIndulgenceEmailer : IIndulgenceEmailer
    {
        private Amazon.SimpleEmail.AmazonSimpleEmailService service;
        public void Send(Indulgence indulgence, string indulgenceFilePath)
        {
            service =
                Amazon.AWSClientFactory.CreateAmazonSimpleEmailServiceClient(
                    ConfigurationManager.AppSettings["awsAccessKeyId"],
                    ConfigurationManager.AppSettings["awsAccessSecret"]);

            SendEmailRequest request = new SendEmailRequest();
            request.WithDestination(BuildDestination());
            request.WithMessage(BuildMessage(indulgence));
            request.WithSource("andrew.myhre@gmail.com");

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
            subject.WithCharset("utf-8");
            subject.WithData(subjectText);

            Content html = new Content();
            html.WithCharset("UTF-8");
            html.WithData(htmlBody);

            Content text = new Content();
            text.WithCharset("UTF-8");
            text.WithData(textBody);

            Body body = new Body();
            body.WithHtml(html);
            body.WithText(text);

            Message message = new Message();
            message.WithBody(body);
            message.WithSubject(subject);

            return message;
        }

        private Destination BuildDestination()
        {
            Destination destination = new Destination();
            destination.WithToAddresses("andrew.myhre@gmail.com");
            return destination;
        }
    }
}