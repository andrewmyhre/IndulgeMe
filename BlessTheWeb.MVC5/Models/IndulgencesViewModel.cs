using BlessTheWeb.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlessTheWeb.MVC5.Models
{
    public class IndulgencesViewModel
    {
        public Indulgence Indulgence { get; set; }
        public Sin Sin { get; set; }
        public decimal TotalDonated { get; set; }
        public int TotalDonationCount { get; set; }
        public string ImageAlt { get; set; }
    }
}