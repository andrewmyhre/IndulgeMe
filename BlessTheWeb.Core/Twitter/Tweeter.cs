using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;


namespace BlessTheWeb.Core.Twitter
{
    public interface ITweeter
    {
        void Tweet(Indulgence indulgence);
    }

    public class Tweeter : ITweeter
    {
        private readonly ILog _logger;

        public Tweeter(ILog logger)
        {
            _logger = logger;
        }

        public void Tweet(Indulgence indulgence)
        {
            _logger.DebugFormat("tweet {0}", indulgence.Confession);

            string status = "";
            if (!string.IsNullOrWhiteSpace(indulgence.Name))
            {
                status = string.Format("Bless you {0}, your sin has been absolved {1}/indulgence/{2}",
                    indulgence.Name,
                    ConfigurationManager.AppSettings["WebsiteHostName"],
                    indulgence.Guid);
            }
            else
            {
                status = string.Format("A sin has been absolved {0}/indulgence/{1}",
                    ConfigurationManager.AppSettings["WebsiteHostName"],
                    indulgence.Guid);
            }

            Auth.SetUserCredentials(ConfigurationManager.AppSettings["Twitter_ConsumerKey"], 
                ConfigurationManager.AppSettings["Twitter_ConsumerSecret"], 
                ConfigurationManager.AppSettings["Twitter_Token"], 
                ConfigurationManager.AppSettings["Twitter_TokenSecret"]);

            try
            {
                Tweetinvi.Tweet.PublishTweet(status);
            }
            catch (Exception ex)
            {
                _logger.Warn("Could not send tweet", ex);
            }
        }
}
}
