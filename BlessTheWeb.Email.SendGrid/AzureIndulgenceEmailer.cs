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
using Newtonsoft.Json;

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
                dynamic sg = new SendGrid.SendGridAPIClient(_sendGridApiKey);
                var fromAddress = ConfigurationManager.AppSettings["SendGridFromAddress"];
                var fromName = ConfigurationManager.AppSettings["SendGridFromName"];
                var indulgenceLink = string.Format("{0}/indulgence/pdf/{1}",
                                    ConfigurationManager.AppSettings["WebsiteHostName"], indulgence.Guid);
                string data = @"{
'content': [
    {
      'type': 'text/html', 
      'value': '<html><p>Hello good person,</p><p>Please accept this *genuine plenary indulgence. Be sure to present it when you are received in purgatory.</p><p><a href=\'" + indulgenceLink + @"\'>Click here for the PDF version</a></p><p>Send this to your friends, gloat about it, and encourage them to perform good acts too!</p><p><small>*not endorsed by the Catholic church</small></p></html>'
    }
  ],
'from': {
    'email': '"+ fromAddress+ @"', 
    'name': '" + fromName + @"'
  },
'personalizations': 
[
    {
        'to': [{
          'email': '" + indulgence.DonorEmailAddress + @"', 
          'name': '" + (string.IsNullOrWhiteSpace(indulgence.DonorEmailAddress) ? "" : indulgence.DonorEmailAddress) + @"'
        }]
    }
], 
  'reply_to': {
    'email': '" + fromAddress + @"', 
    'name': '" + fromName + @"'
  }, 
  'subject': 'Bless you! Please accept your indulgence'
}";

                Object json = JsonConvert.DeserializeObject<Object>(data);
                data = json.ToString();
                dynamic response = await sg.client.mail.send.post(requestBody: data);

                _log.InfoFormat("sent an email to " + indulgence.DonorEmailAddress);
                _log.InfoFormat("response code: {0}, result: {1}, headers: {2}",
                    response.StatusCode, response.Body.ReadAsStringAsync().Result, response.Headers.ToString());
        }
    }
}
