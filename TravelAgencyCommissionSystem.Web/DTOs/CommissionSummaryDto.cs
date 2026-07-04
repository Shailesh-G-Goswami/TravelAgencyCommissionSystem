namespace TravelAgencyCommissionSystem.Web.DTOs
{
    public class CommissionSummaryDto
    {
        public int AgentId { get; set; }

        public string AgentName { get; set; } = string.Empty;

        public decimal TotalConfirmedSales { get; set; }

        public decimal TotalReservedCommission { get; set; }

        public string MostFrequentTier { get; set; } = "N/A";
    }
}
