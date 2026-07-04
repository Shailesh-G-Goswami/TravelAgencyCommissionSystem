using Microsoft.EntityFrameworkCore;
using TravelAgencyCommissionSystem.Web.Data;
using TravelAgencyCommissionSystem.Web.Models;
using TravelAgencyCommissionSystem.Web.Services.Interfaces;

namespace TravelAgencyCommissionSystem.Web.Services
{
    public class CommissionService : ICommissionService
    {
        private readonly ApplicationDbContext _context;

        public CommissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetApplicableRateAsync(Agent agent, decimal monthlySalesIncludingCurrentBooking)
        {
            // Explicit override (including 0%)
            if(agent.TierOverrideRate.HasValue)
            {
                return agent.TierOverrideRate.Value;
            }

            var tier = await _context.CommissionTiers
                .AsNoTracking()
                .FirstOrDefaultAsync(t =>
                monthlySalesIncludingCurrentBooking >= t.MinMonthlySales &&
                (t.MaxMonthlySales == null ||
                monthlySalesIncludingCurrentBooking <= t.MaxMonthlySales));

            if(tier is null)
            {
                throw new InvalidOperationException("No commission tier found for the specified sales amount.");
            }

            return tier.RatePercent;
        }

        public async Task<decimal> CalculateCommissionAsync(Agent agent, decimal ticketAmount, decimal monthlySalesIncludingCurrentBooking)
        {
            var rate = await GetApplicableRateAsync(agent, monthlySalesIncludingCurrentBooking);

            return Math.Round(ticketAmount * rate / 100m, 2, MidpointRounding.AwayFromZero);
        }
    }
}
