using System;
using System.Collections.Generic;
using BlessTheWeb.Core;

namespace BlessTheWeb.Models
{
    public class HomeViewModel
    {
        public IEnumerable<Sin> Sins { get; set; }
        public IEnumerable<Sin> TopRedeemed { get; set; }

        public IEnumerable<Indulgence> BlessedIndulgences { get; set; }

        public SiteSummaryInfo SiteInfo { get; set; }
    }
}