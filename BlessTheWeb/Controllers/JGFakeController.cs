using System.Web.Mvc;

namespace BlessTheWeb.Controllers
{
    public class JGFakeController : Controller
    {
        //
        // GET: /JGFake/

        public ActionResult Index(int id)
        {
            string exitUrl = Request["exitUrl"];
            exitUrl = exitUrl.Replace("donationid=JUSTGIVING-DONATION-ID", "");
            ViewData["exiturl"] = exitUrl;
            return View();
        }

    }
}
