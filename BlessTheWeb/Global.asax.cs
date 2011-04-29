using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;
using BlessTheWeb.Core;
using EmailProcessing;
using log4net;
using Raven.Client.Document;
using Raven.Client;

namespace JustConfessing
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private const string RavenSessionKey = "RavenMVC.Session";
        private static DocumentStore documentStore;
        public static decimal TotalDonated { get; set; }
        public static EmailFacade EmailFacade { get; private set; }
        private static ILog _logger = LogManager.GetLogger("MvcApplication");

        public static IDocumentSession CurrentSession
        {
            get { return (IDocumentSession)HttpContext.Current.Items[RavenSessionKey]; }
        }

        public MvcApplication()
        {
            //Create a DocumentSession on BeginRequest  
            //create a document session for every unit of work
            BeginRequest += (sender, args) =>
                HttpContext.Current.Items[RavenSessionKey] = documentStore.OpenSession();
            //Destroy the DocumentSession on EndRequest
            EndRequest += (o, eventArgs) =>
            {
                var disposable = HttpContext.Current.Items[RavenSessionKey] as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            };
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Sdi",
                            "sdi/{action}/{id}",
                            new {controller = "Sdi", action = "Index"});

            routes.MapRoute(
                "Indulgence List",
                "indulgences",
                new {controller = "Indulgence", action = "List"});

            routes.MapRoute(
                "Indulgence Detail",
                "indulgences/{id}",
                new {controller = "Indulgence", action = "Index"});

            routes.MapRoute(
                "Sins List",
                "sins/list",
                new {controller = "Sins", action = "List"});

            routes.MapRoute(
                "Sin Detail",
                "sins/{id}",
                new {controller = "Sins", action = "Index"});

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            //documentStore = new DocumentStore { DataDirectory=System.Web.Hosting.HostingEnvironment.MapPath("~/App_data") };
            documentStore = new DocumentStore {Url = "Http://localhost:8080"};
            documentStore.Initialize();

            using (var session = documentStore.OpenSession())
            {
                new RavenIndexManager().SetUpIndexes(session);

                //BackgroundWorker bw = new BackgroundWorker();
                //bw.DoWork += delegate { AggregateSins(session); };
                //bw.RunWorkerAsync();

                // get total donated
                TotalDonated = session.Query<Indulgence>("BlessedIndulgences").ToList().Sum(a => a.AmountDonated);
            }

            EmailFacade = EmailFacadeFactory.CreateFromConfiguration();
            EmailFacade.LoadTemplates();

        }

        protected void Application_Error()
        {
            Exception lastException = Server.GetLastError();
            string url = "";
            try
            {
                url = Request.Url.ToString();
            } catch
            {
            }

            _logger.Fatal("Application error at " + (!string.IsNullOrWhiteSpace(url) ? url : "unknown page"), lastException);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
        }

        private static void AggregateSins(IDocumentSession session)
        {
            var sinAggregator = new SinAggregator(session);
            sinAggregator.AddTrawler(new BlessTheWeb.Core.Trawlers.TextsFromLastNightSinTrawler(new WebPageDownloader()));
            sinAggregator.Trawl();
        }
    }
}