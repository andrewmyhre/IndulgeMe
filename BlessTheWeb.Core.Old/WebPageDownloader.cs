using System.Net;
using BlessTheWeb.Core.Trawlers;

namespace BlessTheWeb.Core
{
    public class WebPageDownloader : IWebPageDownloader
    {
        public string GetPage(string url)
        {
            return new WebClient().DownloadString(url);
        }
    }
}