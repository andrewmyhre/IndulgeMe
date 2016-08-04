using BlessTheWeb.Core;

namespace BlessTheWeb.Data.NHibernate
{
    public class IndulgenceMap : FluentNHibernate.Mapping.ClassMap<Indulgence>
    {
        public IndulgenceMap()
        {
            Id(x => x.Id);
            Map(x => x.Guid);
            Map(x => x.AmountDonated);
            Map(x => x.BackgroundImageName);
            Map(x => x.CharityId);
            Map(x => x.CharityName);
            Map(x => x.Confession);
            Map(x => x.DateConfessed);
            Map(x => x.DonationReference);
            Map(x => x.DonorEmailAddress);
            Map(x => x.IsBlessed);
            Map(x => x.IsConfession);
            Map(x => x.JustGivingDonationId);
            Map(x => x.Name);
            References(x => x.Sin);
            Map(x => x.Tweeted);
        }
    }
}