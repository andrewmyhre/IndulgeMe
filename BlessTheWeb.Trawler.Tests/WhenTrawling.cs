using System.IO;
using System.Linq;
using BlessTheWeb.Core.Trawlers;
using Moq;
using NUnit.Framework;

namespace BlessTheWeb.Trawler.Tests
{
    [TestFixture]
    public class WhenTrawling
    {
        [Test]
        public void CanFindTextsFromLastNight()
        {
            
            Mock<IWebPageDownloader> fetcherMock = new Mock<IWebPageDownloader>();
            fetcherMock
                .Setup(m => m.GetPage("http://www.textsfromlastnight.com/Texts-From-Worst-Nights-Week.html"))
                .Returns(
                File.ReadAllText("TextFromLastNightSample.txt")
                .Replace(
"<li class=\"next\"><a href=\"/texts-from-last-night/page:2/type:Worst/span:Week\"> Next Page</a></li>",
"")
                );


            var trawler = new TextsFromLastNightSinTrawler(fetcherMock.Object);

            var texts = trawler.GetSins();

            Assert.That(texts.Sins.Count() == 15);
            Assert.That(texts.Sins.ElementAt(0).Content.Equals("Just wrote the directions to get to the girls house im hooking up with on the back of my marriage certificate. Officially worst husband ever."));
        }

        [Test]
        public void CanParseIdFromTFLN()
        {
            Mock<IWebPageDownloader> fetcherMock = new Mock<IWebPageDownloader>();
            fetcherMock
                .Setup(m => m.GetPage("http://www.textsfromlastnight.com/Texts-From-Worst-Nights-Week.html"))
                .Returns(
                File.ReadAllText("TextFromLastNightSample.txt")
                .Replace(
"<li class=\"next\"><a href=\"/texts-from-last-night/page:2/type:Worst/span:Week\"> Next Page</a></li>",
"")
                );
            var trawler = new TextsFromLastNightSinTrawler(fetcherMock.Object);
            var texts = trawler.GetSins();

            Assert.That(texts.Sins.ElementAt(0).SourceSinId, Is.EqualTo("20582"));
        }

        [Test]
        public void WillDownloadAllPagesInSequence()
        {
            Mock<IWebPageDownloader> fetcherMock = new Mock<IWebPageDownloader>();
            fetcherMock
                .Setup(m => m.GetPage("http://www.textsfromlastnight.com/Texts-From-Worst-Nights-Week.html"))
                .Returns(File.ReadAllText("TextFromLastNightSample.txt"));

            fetcherMock
    .Setup(m => m.GetPage("http://www.textsfromlastnight.com/texts-from-last-night/page:2/type:Worst/span:Week"))
    .Returns(
        File.ReadAllText("TextFromLastNightSample.txt")
        .Replace(
        "<li class=\"next\"><a href=\"/texts-from-last-night/page:2/type:Worst/span:Week\"> Next Page</a></li>",
        "<li class=\"next\"><a href=\"/texts-from-last-night/page:3/type:Worst/span:Week\"> Next Page</a></li>"));

            fetcherMock
.Setup(m => m.GetPage("http://www.textsfromlastnight.com/texts-from-last-night/page:3/type:Worst/span:Week"))
.Returns(
File.ReadAllText("TextFromLastNightSample.txt")
.Replace("<li class=\"next\"><a href=\"/texts-from-last-night/page:2/type:Worst/span:Week\"> Next Page</a></li>",""));

            
            var trawler = new TextsFromLastNightSinTrawler(fetcherMock.Object);

            var result = trawler.GetSins();

            Assert.That(result.Sins.Count(), Is.EqualTo(45));
        }
    }
}