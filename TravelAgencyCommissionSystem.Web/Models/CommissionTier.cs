namespace TravelAgencyCommissionSystem.Web.Models
{
    public class CommissionTier
    {
        public int Id { get; set; }

        public decimal MinMonthlySales { get; set; }

        public decimal? MaxMonthlySales { get; set; }

        public decimal RatePercent { get; set; }
    }
}
