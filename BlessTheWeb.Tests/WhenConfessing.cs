using System;
using System.IO;
using BlessTheWeb.Core;
using NUnit.Framework;
using Raven.Client.Document;

namespace BlessTheWeb.Tests
{
    [TestFixture]
    public class WhenConfessing
    {
        [Test]
        public void CanSaveAConfession()
        {
            // arrange
            Indulgence indulgence = new Indulgence();
            indulgence.Confession = "I am a bad bad boy";

            using (var store = new DocumentStore() {Url="http://localhost:8080"})
            {
                store.Initialize();
                using (var session = store.OpenSession())
                {
                    session.Store(indulgence);
                }
            }

            // assert
            Assert.That(!string.IsNullOrWhiteSpace(indulgence.Id));
        }

        [Test]
        public void CAnSaveAndRetrieveAConfession()
        {
            // arrange
            Indulgence actual = null;
            Indulgence indulgence = new Indulgence();
            indulgence.Confession = "I am a bad bad boy";

            using (var store = new DocumentStore() { Url = "http://localhost:8080" })
            {
                store.Initialize();
                using (var session = store.OpenSession())
                {
                    session.Store(indulgence);
                    actual = session.Load<Indulgence>(indulgence.Id);
                }
            }

            // assert
            Assert.That(!string.IsNullOrWhiteSpace(indulgence.Id));
            Assert.That(actual != null);
            Assert.That(actual.Id == indulgence.Id);
            Assert.That(actual.Confession == indulgence.Confession);
        }

        [Test]
        public void ShouldGenerateAnIndulgence()
        {
            var indulgenceGenerator = new IndulgenceGenerator();
            indulgenceGenerator.Generate(new Indulgence(), Environment.CurrentDirectory, Environment.CurrentDirectory, "Marie Curie Cancer Society", Environment.CurrentDirectory, Environment.CurrentDirectory);

            Assert.That(File.Exists(Path.Combine(Environment.CurrentDirectory, "indulgences\\1.png")));
        }
    }
}
