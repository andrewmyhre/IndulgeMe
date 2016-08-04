using System.Collections.Generic;

namespace BlessTheWeb.Core.Trawlers
{
    public class TrawlerResult
    {
        public IEnumerable<Sin> Sins { get; set; }
        public bool HasNextPage { get; set; }
        public string NextPageUrl { get; set; }
        public TrawlerResult()
        {
            Sins = new List<Sin>();
        }
    }
}