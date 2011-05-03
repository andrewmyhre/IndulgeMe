using BlessTheWeb.Core;

namespace BlessTheWeb.Models
{
    public class IndulgencesViewModel
    {
        public Indulgence Indulgence { get; set; }
        public Sin Sin { get; set; }
        public decimal TotalDonated { get; set; }
        public int TotalDonationCount { get; set; }
        public string ImageAlt { get; set; }
    }
}