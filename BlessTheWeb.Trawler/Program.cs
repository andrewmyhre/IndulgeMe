using System;
using System.Collections.Generic;
using System.Linq;
using BlessTheWeb.Core;
using BlessTheWeb.Core.Trawlers;
using log4net;
using BlessTheWeb.Core.Repository;
using Ninject;

namespace BlessTheWeb.Trawler
{
    class Program
    {
        private static IDatabase _db = null;

        private static ILog log = LogManager.GetLogger("program");
        static IEnumerable<ISinTrawler> _trawlers = null;

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            var kernel = new StandardKernel();
            kernel.Bind<ISinTrawler>().To<TextsFromLastNightSinTrawler>();
            kernel.Bind<IWebPageDownloader>().To<WebPageDownloader>();
            kernel.Bind<IDatabase>().To<InMemoryDatabase>();
            kernel.Bind<IDatabaseSession>().To<InMemoryDatabaseSession>();

            _db = kernel.Get<IDatabase>();
            _trawlers = kernel.GetAll<ISinTrawler>();

            foreach (var trawler in _trawlers)
            {
                log.DebugFormat("Trawling sins from {0}...", trawler.SourceName);
                var sins = trawler.GetSins();
                log.DebugFormat("Persisting {0} sins...", sins.Sins.Count());
                StoreSins(sins);
                log.Debug("Done");
            }
        }

        private static void StoreSins(TrawlerResult sins)
        {
            using (var session = _db.OpenSession())
            {
                foreach (var sin in sins.Sins)
                {
                    try
                    {
                        session.Store(sin);
                        log.DebugFormat("Stored: {0}", sin.Content);
                    }
                    catch (Exception ex)
                    {
                        log.Fatal("fail", ex);
                        Environment.Exit(-1);
                    }
                }
                session.SaveChanges();
            }
        }
    }
}
