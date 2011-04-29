using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlessTheWeb.Core;
using BlessTheWeb.Core.Extensions;
using JustConfessing;
using Raven.Client;

namespace BlessTheWeb.Controllers
{
    public class SinsController : ControllerBase
    {
        //
        // GET: /Sins/

        public ActionResult List()
        {
            var viewModel = new BlessTheWeb.Models.SinsViewModel();
            viewModel.Sins = MvcApplication.CurrentSession.Query<Sin>("AllSins").ToList();

            return View(viewModel);
        }

        public ActionResult Index(int id)
        {
            var viewModel = new BlessTheWeb.Models.SinDetailViewModel();
            viewModel.Sin = MvcApplication.CurrentSession.Load<Sin>(id.ToRavenDbId("sins"));
            viewModel.Absolutions =
                MvcApplication.CurrentSession.LuceneQuery<Indulgence>("AllIndulgences").WhereEquals("SinId",
                                                                                                    viewModel.Sin.Id);

            return View(viewModel);
        }

    }
}

namespace BlessTheWeb.Models
{
    public class SinsViewModel
    {
        public IEnumerable<Sin> Sins { get; set; }
    }

    public class SinDetailViewModel
    {
        public Sin Sin { get; set; }
        public IEnumerable<Indulgence> Absolutions { get; set; }
    }
}
