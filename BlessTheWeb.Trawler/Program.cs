using System;
using System.Collections.Generic;
using System.Linq;
using BlessTheWeb.Core;
using BlessTheWeb.Core.Trawlers;
using log4net;
using BlessTheWeb.Core.Repository;
using Ninject;
using BlessTheWeb.Data.NHibernate;

namespace BlessTheWeb.Trawler
{
    class Program
    {
        private static ILog log = LogManager.GetLogger("program");
        private static IIndulgeMeService _indulgeMeService;
        static IEnumerable<ISinTrawler> _trawlers = null;

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            var kernel = new StandardKernel();
            kernel.Bind<ISinTrawler>().To<TextsFromLastNightSinTrawler>();
            kernel.Bind<IWebPageDownloader>().To<WebPageDownloader>();
            kernel.Bind<IIndulgeMeService>().To<NHibernateIndulgeMeService>();

            _trawlers = kernel.GetAll<ISinTrawler>();
            _indulgeMeService = kernel.Get<IIndulgeMeService>();

            foreach (var trawler in _trawlers)
            {
                log.DebugFormat("Trawling sins from {0}...", trawler.SourceName);
                var sins = trawler.GetSins();
                log.DebugFormat("Persisting {0} sins...", sins.Sins.Count());
                _indulgeMeService.SaveSins(sins.Sins);
                log.Debug("Done");
            }
        }
    }
}
