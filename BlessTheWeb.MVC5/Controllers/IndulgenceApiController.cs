using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BlessTheWeb.Core;
using BlessTheWeb.MVC5.Models;

namespace BlessTheWeb.MVC5.Controllers
{
    public class IndulgenceApiController : ApiController
    {
        private readonly IIndulgeMeService _indulgeMeService;

        public IndulgenceApiController(IIndulgeMeService indulgeMeService)
        {
            _indulgeMeService = indulgeMeService;
        }

        [HttpGet]
        public IEnumerable<IndulgenceViewModel> GetLatest()
        {
            var indulgences = _indulgeMeService.AllIndulgences(0, 10);
            var vm = indulgences.Select(
                i => new IndulgenceViewModel()
                {
                    AmountDonated = i.AmountDonated.ToString("c"),
                    CharityName = i.CharityName,
                    Confession = i.Confession,
                    Date = i.DateConfessed.ToString("dd/MM/yyyy hh:mm"),
                    Id = i.Id.ToString(),
                    ThumbnailUrl=this.Url.Link("ViewIndulgenceImage", new { guid=i.Guid, size=3}),
                    Guid=i.Guid
                });

            return vm.ToArray();
        }

    }
}
