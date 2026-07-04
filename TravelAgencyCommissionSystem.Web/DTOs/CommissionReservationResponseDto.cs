namespace TravelAgencyCommissionSystem.Web.DTOs
{
    public class CommissionReservationResponseDto
    {
        public int ReservationId { get; set; }

        public int BookingId { get; set; }

        public decimal ReservedAmount { get; set; }

        public decimal RateApplied { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
