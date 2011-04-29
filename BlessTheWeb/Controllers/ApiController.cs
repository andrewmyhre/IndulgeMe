using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using BlessTheWeb.Core;
using BlessTheWeb.Core.Extensions;
using BlessTheWeb.Models;
using JustGiving.Api.Sdk;

namespace BlessTheWeb.Controllers
{
    public class ApiController : ControllerBase
    {
        //
        // GET: /Api/
        [OutputCache(VaryByParam="q", Duration=10000)]
        public ActionResult FindCharities(string q)
        {
            var config = new ClientConfiguration(
                "https://api.staging.justgiving.com/", 
                ConfigurationManager.AppSettings["JGApiKey"],
                1);
            var client = new JustGivingClient(config);
            var results = client.Search.CharitySearch(q, 1, 10);

            return Json(results.Results, JsonRequestBehavior.AllowGet);
        }

        [OutputCache(Duration=6000, VaryByParam="q")]
        public JsonResult GetLatest()
        {
            var indulgences = _indulgeMeService.AllIndulgences(0, 10);

            AutoMapper.Mapper.CreateMap<Indulgence, IndulgenceViewModel>()
                .ForMember(d => d.Date, m => m.MapFrom(s => s.DateConfessed.ToString("dd/MM/yyyy hh:mm")))
                .ForMember(d => d.Id, m => m.MapFrom(s => s.Id.IdValue()))
                .ForMember(d=>d.AmountDonated, m=>m.MapFrom(s=>s.AmountDonated.ToString("c")));

            var viewModel = (from i in indulgences select AutoMapper.Mapper.Map<Indulgence,IndulgenceViewModel>(i));
            
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

    }
}
