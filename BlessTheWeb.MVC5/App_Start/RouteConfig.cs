using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BlessTheWeb.MVC5
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "BeginConfession",
                url: "bless-me-father-for-i-have-sinned",
                defaults: new { controller = "Home", action = "BeginConfession" }
            );

            routes.MapRoute(
                name: "Confess",
                url: "confess/{guid}",
                defaults: new { controller = "Home", action = "Confess" }
            );

            routes.MapRoute(
                name: "EnterDonationProcess",
                url: "five-hail-marys",
                defaults: new {controller = "Home", action = "EnterDonationProcess"});

            routes.MapRoute(
                name: "Confessed",
                url: "confessed/{guid}",
                defaults: new { controller = "Home", action = "ChooseACharity" }
            );


            routes.MapRoute(
                name: "Absolve",
                url: "absolve/{id}",
                defaults: new { controller = "Home", action = "ChooseACharity" }
            );

            routes.MapRoute(
                name: "ViewIndulgence",
                url: "indulgence/{guid}",
                defaults: new { controller = "Indulgence", action = "Index" }
            );
            routes.MapRoute(
                name: "ViewIndulgenceImage",
                url: "indulgence/image/{guid}/{size}",
                defaults: new { controller = "Indulgence", action = "Image" }
            );
            routes.MapRoute(
                name: "ViewIndulgencePdf",
                url: "indulgence/pdf/{guid}",
                defaults: new { controller = "Indulgence", action = "Pdf" }
            );
            routes.MapRoute(
                name: "ListIndulgences",
                url: "indulgences/list/{page}",
                defaults: new { controller = "Indulgence", action = "List", page = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SinsList",
                url: "sins/list/{page}",
                defaults: new { controller = "Sins", action = "Index", page = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SdiReturn",
                url: "sdi/return/{guid}",
                defaults: new { controller = "Sdi", action = "Return" }
            );

            routes.MapRoute(
                name: "FakeSdi",
                url: "sdi/fake",
                defaults: new { controller = "Sdi", action = "FakeSdi" }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
