using System.ComponentModel.DataAnnotations;

namespace TravelAgencyCommissionSystem.Web.Models
{
    public class CommissionReservation
    {
        public int Id { get; set; }

        public int BookingId { get; set; }

        public decimal ReservedAmount { get; set; }

        public decimal RateApplied { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsVoided { get; set; }

        public DateTime? VoidedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public Booking Booking { get; set; } = null!;
    }
}
