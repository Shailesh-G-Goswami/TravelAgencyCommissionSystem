using System.Runtime.Intrinsics.X86;

namespace TravelAgencyCommissionSystem.Web.Models
{
    public class Agent
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // Null = use tier calculation
        // 0 = explicit 0% commission
        public decimal? TierOverrideRate { get; set; }

        public ICollection<Booking> Bookings { get; set; }
            = new List<Booking>();
    }
}
