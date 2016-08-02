using BlessTheWeb.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlessTheWeb.MVC5.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Sin> Sins { get; set; }
        public IEnumerable<Sin> TopRedeemed { get; set; }

        public IEnumerable<Indulgence> BlessedIndulgences { get; set; }

        public SiteSummaryInfo SiteInfo { get; set; }
    }
}