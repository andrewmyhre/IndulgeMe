using BlessTheWeb.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BlessTheWeb.MVC5.Controllers
{
    public class SinsController : BlessTheWebController
    {
        private readonly IIndulgeMeService _indulgeMeService;

        public SinsController(IIndulgeMeService indulgeMeService)
        {
            _indulgeMeService = indulgeMeService;
        }

        // GET: Sins
        public ActionResult Index()
        {
            int page = 1, pageSize = 100;
            var sins = _indulgeMeService.GetSinsByDonationAmount(page, pageSize);
            return View(sins);
        }
    }
}