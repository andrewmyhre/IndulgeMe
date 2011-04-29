using System.Collections.Generic;
using log4net;

namespace BlessTheWeb.Core.Trawlers
{
    public class TextsFromLastNightSinTrawler : ISinTrawler
    {
        private const string InitialPageUrl = "http://www.textsfromlastnight.com/Texts-From-Worst-Nights-Week.html";
        private const string StartTag = "<a href=\"/Text-Replies-";
        private const string EndOfStartTag = "\">";
        private const string EndTag = "</a>";
        private const string EndOfId = ".html";
        private const string NextLinkStartText = "<li class=\"next\"><a href=\"";
        private const string NextLinkEndtext = "\"> Next Page";
        private const string BaseDomain = "http://www.textsfromlastnight.com";


        private ILog log = LogManager.GetLogger("TextsFromLastNightSinTrawler");
        private readonly IWebPageDownloader _pageDownloader;

        public string SourceName
        {
            get { return "TextsFromLastNight"; }
        }

        public TextsFromLastNightSinTrawler(IWebPageDownloader pageDownloader)
        {
            _pageDownloader = pageDownloader;
        }

        public TrawlerResult GetSins()
        {
            log.DebugFormat("Fetch:{0}", InitialPageUrl);
            string pageData = _pageDownloader.GetPage(InitialPageUrl);

            var trawlerResult = new TrawlerResult();
            var allTexts = new List<Sin>();
            var result = ParseTextsInPage(pageData);
            allTexts.AddRange(result.Sins);
            while(result.HasNextPage)
            {
                log.DebugFormat("Fetch:{0}", result.NextPageUrl);
                pageData =
                _pageDownloader.GetPage(result.NextPageUrl);

                result = ParseTextsInPage(pageData);
                allTexts.AddRange(result.Sins);
            }

            trawlerResult.Sins = allTexts;

            return trawlerResult;
        }



        private TrawlerResult ParseTextsInPage(string pageData)
        {
            var result = new TrawlerResult();

            int index = pageData.IndexOf(StartTag);
            while (index >= 0)
            {
                int startTagEnd = pageData.IndexOf(EndOfStartTag, index) + 2;
                int end = pageData.IndexOf(EndTag, startTagEnd);
                string message = pageData.Substring(startTagEnd, end - startTagEnd);
                int idStart = index + StartTag.Length;
                int idEnd = pageData.IndexOf(EndOfId, idStart);
                string textId = pageData.Substring(idStart, idEnd - idStart);
                ((List<Sin>)result.Sins).Add(new Sin() { Content = message, SourceSinId = textId, Source = SourceName });
                index = pageData.IndexOf(StartTag, end);
            }

            int nextPageLinkStart =
                pageData.IndexOf(NextLinkStartText) + NextLinkStartText.Length;
            if (nextPageLinkStart >= NextLinkStartText.Length)
            {
                int nextPageLinkEnd = pageData.IndexOf(NextLinkEndtext, nextPageLinkStart);
                string nextPageUrl = pageData.Substring(nextPageLinkStart, nextPageLinkEnd - nextPageLinkStart);
                result.HasNextPage = true;
                result.NextPageUrl = BaseDomain+nextPageUrl;
            }
            else
            {
                result.HasNextPage = false;
            }
            return result;
        }
    }
}