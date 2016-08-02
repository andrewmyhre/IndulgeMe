using BlessTheWeb.MVC5.Models;
using JustGiving.Api.Sdk;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BlessTheWeb.Core;
using BlessTheWeb.Core.Repository;

namespace BlessTheWeb.MVC5.Controllers
{

    [Route("api/charities")]
    public class CharitiesController : ApiController
    {
        protected readonly IIndulgeMeService _indulgeMeService;
        public CharitiesController(IIndulgeMeService indulgeMeService)
        {
            _indulgeMeService = indulgeMeService;
        }
        public class BtwCharitySearchResult
        {
            public string Description { get; internal set; }
            public int Id { get; internal set; }
            public string Logo { get; internal set; }
            public string Name { get; internal set; }
        }

        [HttpGet]
        public IEnumerable<BtwCharitySearchResult> FindCharities(string q)
        {
            var config = new ClientConfiguration(
                ConfigurationManager.AppSettings["JgApiBaseUrl"],
                ConfigurationManager.AppSettings["JGApiKey"],
                1);

            var client = new JustGivingClient(config);
            var response = client.Search.CharitySearch(q);

            return response.Results
                .Select(r=>
                new BtwCharitySearchResult()
                {
                    Id=int.Parse(r.CharityId),
                    Name=r.Name,
                    Logo=r.LogoFileName,
                    Description=r.Description
                });
        }

        public IEnumerable<IndulgenceViewModel> LatestIndulgences()
        {
            var indulgences = _indulgeMeService.AllIndulgences(0, 10);
            var vm = indulgences.Select(
                i => new IndulgenceViewModel()
                {
                    AmountDonated = i.AmountDonated.ToString("c"),
                    CharityName = i.CharityName,
                    Confession = i.Confession,
                    Date = i.DateConfessed.ToString("dd/MM/yyyy hh:mm"),
                    Id = i.Id.ToString()
                });

            return vm.ToArray();
        }
    }
}
