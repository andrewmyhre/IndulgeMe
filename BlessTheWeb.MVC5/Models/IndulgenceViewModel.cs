using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlessTheWeb.MVC5.Models
{
    public class IndulgenceViewModel
    {
        public string Id { get; set; }
        public string Confession { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string AmountDonated { get; set; }
        public string CharityName { get; set; }
        public string ThumbnailUrl { get; internal set; }
    }
}