using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlessTheWeb.Core.Trawlers;
using log4net;
using BlessTheWeb.Core.Repository;

namespace BlessTheWeb.Core
{
    public class SinAggregator
    {
        private IDatabase _db = null;

        private ILog log = LogManager.GetLogger("SinAggregator");
        List<ISinTrawler> _trawlers = new List<ISinTrawler>();

        public SinAggregator(IDatabase db)
        {
            _db = db;
        }

        public void AddTrawler(ISinTrawler trawler)
        {
            _trawlers.Add(trawler);
        }

        public void Trawl()
        {
            using (var session = _db.OpenSession())
            {
                foreach (var trawler in _trawlers)
                {
                    log.DebugFormat("Trawling sins from {0}...", trawler.SourceName);
                    var sins = trawler.GetSins();
                    log.DebugFormat("Persisting {0} sins...", sins.Sins.Count());
                    StoreSins(session,sins);
                    log.Debug("Writing to database...");
                    session.SaveChanges();
                    log.Debug("Done");
                }
            }
        }

        private void StoreSins(IDatabaseSession session, TrawlerResult sins)
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
        }
    }
}
