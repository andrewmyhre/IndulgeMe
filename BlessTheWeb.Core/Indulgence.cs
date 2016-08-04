using System;

namespace BlessTheWeb.Core
{
    public class Indulgence
    {
        public virtual int Id { get; set; }
        public virtual string Confession { get; set;}
        public virtual DateTime DateConfessed { get; set; }
        public virtual bool IsBlessed { get; set; }
        public virtual bool IsConfession { get; set; }
        public virtual Sin Sin { get; set; }
        public virtual int CharityId { get; set; }
        public virtual decimal AmountDonated { get; set; }
        public virtual string Name { get; set;}
        public virtual int JustGivingDonationId { get; set; }
        public virtual string DonationReference { get; set; }
        public virtual bool Tweeted { get; set; }
        public virtual string DonorEmailAddress { get; set; }
        public virtual string CharityName { get; set; }
        public virtual Guid Guid { get; set; }
        public virtual string BackgroundImageName { get; set; }
    }
}