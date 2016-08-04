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
        private readonly IIndulgeMeService _indulgeMeService;
        private ILog log = LogManager.GetLogger("SinAggregator");
        List<ISinTrawler> _trawlers = new List<ISinTrawler>();

        public SinAggregator(IIndulgeMeService indulgeMeService)
        {
            _indulgeMeService = indulgeMeService;
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
                log.Debug("Done");
            }
        }

        private void StoreSins(TrawlerResult sins)
        {
            _indulgeMeService.SaveSins(sins.Sins);
        }
    }
}
