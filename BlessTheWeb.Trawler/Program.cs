using System;
using System.Collections.Generic;
using System.Linq;
using BlessTheWeb.Core;
using BlessTheWeb.Core.Trawlers;
using log4net;
using Raven.Client;
using Raven.Client.Document;

namespace BlessTheWeb.Trawler
{
    class Program
    {
        private static IDocumentStore _ravenStore = null;
        private static IDocumentSession _ravenSession = null;

        private static ILog log = LogManager.GetLogger("program");
        static List<ISinTrawler> _trawlers = new List<ISinTrawler>();

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            log.Debug("Initialising store...");
            InitializeRaven();

            _trawlers.Add(new TextsFromLastNightSinTrawler(new WebPageDownloader()));

            foreach (var trawler in _trawlers)
            {
                log.DebugFormat("Trawling sins from {0}...", trawler.SourceName);
                var sins = trawler.GetSins();
                log.DebugFormat("Persisting {0} sins...", sins.Sins.Count());
                StoreSins(sins);
                log.Debug("Writing to database...");
                _ravenSession.SaveChanges();
                log.Debug("Done");
            }

            _ravenSession.Dispose();
        }

        private static void StoreSins(TrawlerResult sins)
        {
            foreach (var sin in sins.Sins)
            {
                try
                {
                    _ravenSession.Store(sin);
                    log.DebugFormat("Stored: {0}", sin.Content);
                }
                catch (Exception ex)
                {
                    log.Fatal("fail", ex);
                    Environment.Exit(-1);
                }
            }
        }

        private static void InitializeRaven()
        {
            _ravenStore = new DocumentStore() { Url = "http://localhost:8080" };
            _ravenStore.Initialize();

            new RavenIndexManager().SetUpIndexes(_ravenStore.OpenSession());

            _ravenSession = _ravenStore.OpenSession();
        }
    }
}
