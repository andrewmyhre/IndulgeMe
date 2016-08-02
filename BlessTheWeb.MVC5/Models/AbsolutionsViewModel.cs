using BlessTheWeb.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlessTheWeb.MVC5.Models
{
    public class AbsolutionsViewModel
    {
        public IEnumerable<Indulgence> Indulgences { get; set; }
        public SiteSummaryInfo SiteInfo { get; set; }
        public int Page { get; set; }
        public int NextPage { get; set; }
        public int PreviousPage { get; set; }
        public bool ShowNextPageLink { get; set; }
        public bool ShowPreviousPageLink { get; set; }

        public int CurrentPage { get; set; }
        public int PagingStart { get; set; }
        public int PagingEnd { get; set; }
    }
}