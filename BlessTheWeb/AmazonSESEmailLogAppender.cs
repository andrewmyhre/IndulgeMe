using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using EmailProcessing;
using JustConfessing;
using log4net.Appender;
using log4net.Core;

namespace BlessTheWeb
{
    public class AmazonSESEmailLogAppender : AppenderSkeleton
    {
        private string _fromAddress;
        public string FromAddress { get { return _fromAddress; } set { _fromAddress = value; } }

        protected override void Append(LoggingEvent loggingEvent)
        {
            string logMessage = RenderLoggingEvent(loggingEvent);

            Send(logMessage);
        }

        private void Send(string logMessage)
        {
            MvcApplication.EmailFacade.AddEmailToQueue(
                new string[] { "andrew.myhre@gmail.com" },
                "Exception",
                new Dictionary<string, string>()
                    {
                        {"message", logMessage}
                    },
                new FileInfo[] {  });
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            foreach (LoggingEvent loggingEvent in loggingEvents)
                Send(RenderLoggingEvent(loggingEvent));

            
        }
    }
}