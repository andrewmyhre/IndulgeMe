using System;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

namespace BlessTheWeb.MVC5.Controllers
{
    public class BlessTheWebController : Controller
    {
        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {
            ViewBag.SiteName = ConfigurationManager.AppSettings["SiteName"];
            return base.BeginExecute(requestContext, callback, state);
        }
    }
}