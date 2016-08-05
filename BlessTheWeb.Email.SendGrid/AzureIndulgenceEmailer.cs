using BlessTheWeb.Core;
using BlessTheWeb.Core.Repository;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace BlessTheWeb.Email.Azure
{
    public class AzureIndulgenceEmailer : IIndulgenceEmailer
    {
        private readonly IFileStorage _fileStorage;
        private readonly ILog _log;
        private string _sendGridApiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
        private string _sendGridFromAddress = ConfigurationManager.AppSettings["SendGridFromAddress"];

        public AzureIndulgenceEmailer(IFileStorage fileStorage, ILog log)
        {
            _fileStorage = fileStorage;
            _log = log;

        }

        public async Task Send(Indulgence indulgence, string indulgenceFilePath)
        {
            try
            {
                var indulgenceLink = string.Format("{0}/indulgence/pdf/{1}",
                    ConfigurationManager.AppSettings["WebsiteHostName"], indulgence.Guid);

                ISendGrid message = new SendGridMessage();
                message.From = new MailAddress(_sendGridFromAddress);
                message.To = new MailAddress[] {new MailAddress(indulgence.DonorEmailAddress), };
                message.Subject= "Bless you! Please accept your indulgence";
                message.Html = "<p>hello <a href='"+indulgenceLink+"'>Download your indulgence</a></p>";
                message.Text = "hello";

                var credentials = new NetworkCredential("username", "password");

                // Create an Web transport for sending email.
                var transportWeb = new Web(credentials);

                // Send the email, which returns an awaitable task.
                await transportWeb.DeliverAsync(message);
            }
            catch (Exception ex)
            {
                _log.Error("Error occurred when trying to send an email", ex);
            }
        }
    }
}
