namespace TravelAgencyCommissionSystem.Web.DTOs
{
    public class BookingDetailsDto
    {
        public int Id { get; set; }

        public string AgentName { get; set; } = string.Empty;

        public decimal TicketAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime BookingDate { get; set; }

        public decimal? ReservedAmount { get; set; }

        public decimal? RateApplied { get; set; }

        public bool HasActiveReservation { get; set; }
    }
}
