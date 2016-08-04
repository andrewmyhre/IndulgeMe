using System;
using System.Collections.Generic;

namespace BlessTheWeb.Core
{
    public class Sin
    {
        public virtual string Source { get; set; }
        public virtual string SourceSinId { get; set; }
        public virtual string Content { get; set; }
        public virtual int Id { get; set; }
        public virtual int TotalDonationCount { get; set; }
        public virtual decimal TotalDonated { get; set; }
        public virtual IList<Indulgence> Indulgences { get; set; }
        public virtual Guid Guid { get; set; }
    }
}
