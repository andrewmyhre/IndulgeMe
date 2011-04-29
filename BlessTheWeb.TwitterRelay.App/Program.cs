using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using BlessTheWeb.Core;
using log4net;
using TweetSharp.Twitter.Fluent;

namespace BlessTheWeb.TwitterRelay.App
{
    class Program
    {
        private static ILog logger = LogManager.GetLogger("Program");
        private static FileSystemWatcher outboxWatcher;
        private static string consumerKey, consumerSecret, token, tokenSecret;
        private static ITweetOutbox tweetOutbox;

        static void Main(string[] args)
        {
            Console.Clear();
            log4net.Config.XmlConfigurator.Configure();
            consumerKey = ConfigurationManager.AppSettings["Twitter_ConsumerKey"];
            consumerSecret = ConfigurationManager.AppSettings["Twitter_ConsumerSecret"];
            token = ConfigurationManager.AppSettings["Twitter_Token"];
            tokenSecret = ConfigurationManager.AppSettings["Twitter_TokenSecret"];
            string directory = ConfigurationManager.AppSettings["TweetOutboxDirectory"];
            tweetOutbox = new TweetOutbox(directory);

            outboxWatcher = new FileSystemWatcher(directory);
            outboxWatcher.EnableRaisingEvents = true;
            outboxWatcher.Changed += new FileSystemEventHandler(fsw_Changed);
            Console.WriteLine("Running - press the return key to exit");
            Console.ReadLine();
        }

        static void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            outboxWatcher.EnableRaisingEvents = false;
            ProcessTweets(tweetOutbox, consumerKey, consumerSecret, token, tokenSecret);
            outboxWatcher.EnableRaisingEvents = true;
        }

        private static void ProcessTweets(ITweetOutbox tweetOutbox, string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            try
            {
                tweetOutbox.ProcessAll(SendTweet);
            } catch (Exception ex)
            {
                logger.Fatal(ex);
                Environment.Exit(-1);
            }
        }

        private static void SendTweet(Indulgence indulgence)
        {
            logger.DebugFormat("tweet {0}", indulgence.Confession);

            string status = "";
            status = string.Format("Bless you {0}, your sin has been absolved http://{1}/indulgences/{2}", indulgence.Name,
                                   ConfigurationManager.AppSettings["WebsiteUrlAuthority"], indulgence.Id);

            try
            {
                /*FluentTwitter
                    .CreateRequest()
                    .AuthenticateWith(consumerKey, consumerSecret, token, tokenSecret)
                    .Statuses()
                    .Update(status)
                    .AsJson()
                    .Request();*/
            }
            catch (Exception ex)
            {
                logger.Warn("Could not send tweet", ex);
            }
        }
    }
}
