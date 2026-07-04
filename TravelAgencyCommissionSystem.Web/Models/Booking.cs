using TravelAgencyCommissionSystem.Web.Models.Enums;

namespace TravelAgencyCommissionSystem.Web.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int AgentId { get; set; }

        public decimal TicketAmount { get; set; }

        public DateTime BookingDate { get; set; }

        public BookingStatus Status { get; set; }

        public Agent Agent { get; set; } = null!;

        public CommissionReservation? CommissionReservation { get; set; }
    }
}
