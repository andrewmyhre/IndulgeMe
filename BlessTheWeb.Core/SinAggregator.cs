using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlessTheWeb.Core.Trawlers;
using log4net;
using Raven.Client;

namespace BlessTheWeb.Core
{
    public class SinAggregator
    {
        private IDocumentSession _ravenSession = null;

        private ILog log = LogManager.GetLogger("SinAggregator");
        List<ISinTrawler> _trawlers = new List<ISinTrawler>();

        public SinAggregator(IDocumentSession ravenSession)
        {
            _ravenSession = ravenSession;
        }

        public void AddTrawler(ISinTrawler trawler)
        {
            _trawlers.Add(trawler);
        }

        public void Trawl()
        {
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
        }

        private void StoreSins(TrawlerResult sins)
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
    }
}
