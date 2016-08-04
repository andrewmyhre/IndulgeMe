using BlessTheWeb.Core;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlessTheWeb.Data.NHibernate
{
    public class SinMap : ClassMap<Sin>
    {
        public SinMap()
        {
            Id(x => x.Id);
            Map(x => x.Guid);
            Map(x => x.Content);
            Map(x => x.Source);
            Map(x => x.SourceSinId);
            Map(x => x.TotalDonated);
            Map(x => x.TotalDonationCount);
        }
    }
}
