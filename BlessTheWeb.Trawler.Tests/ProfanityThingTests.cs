using BlessTheWeb.Core;
using NUnit.Framework;

namespace BlessTheWeb.Trawler.Tests
{
    [TestFixture]
    public class ProfanityThingTests
    {
        [Test]
        public void GetsMyProfanity()
        {
            string bad = "eat my fucking shit motherfucker";
            string cleaned = ProfanityThing.CleanRudeWords(bad);

            Assert.That(cleaned, Is.EqualTo("eat my f**king s**t motherf**ker"));

        }
    }
}
