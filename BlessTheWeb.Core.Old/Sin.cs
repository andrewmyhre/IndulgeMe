using System;
using System.Collections.Generic;

namespace BlessTheWeb.Core
{
    public class Sin
    {
        public string Source { get; set; }
        public string SourceSinId { get; set; }
        public string Content { get; set; }
        public object Id { get; set; }
        public int TotalDonationCount { get; set; }
        public decimal TotalDonated { get; set; }
        public IEnumerable<Indulgence> AllAbsolutions { get; set; }
        public Guid Guid { get; set; }
    }
}
