using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlessTheWeb.Core;
using JustConfessing;

namespace BlessTheWeb.Controllers
{
    public class ControllerBase : Controller
    {
        protected readonly IIndulgeMeService _indulgeMeService;
        public ControllerBase(IIndulgeMeService indulgeMeService)
        {
            _indulgeMeService = indulgeMeService;
        }
        public ControllerBase() : this(new IndulgeMeService(MvcApplication.CurrentSession))
        {
            
        }
    }
}