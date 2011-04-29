using System;

namespace BlessTheWeb.Core
{
    public class Indulgence
    {
        public string Id { get; set; }
        public string Confession{ get; set;}
        public DateTime DateConfessed { get; set; }
        public bool IsBlessed { get; set; }
        public bool IsConfession { get; set; }
        public string SinId { get; set; }
        public int CharityId { get; set; }
        public decimal AmountDonated { get; set; }
        public string Name { get; set;}
        public int JustGivingDonationId { get; set; }
        public string DonationReference { get; set; }
        public bool Tweeted { get; set; }
        public string DonorEmailAddress { get; set; }
        public string CharityName { get; set; }
    }
}