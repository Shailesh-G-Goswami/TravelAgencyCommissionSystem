using TravelAgencyCommissionSystem.Web.Models;

namespace TravelAgencyCommissionSystem.Web.Services.Interfaces
{
    public interface ICommissionService
    {
        Task<decimal> GetApplicableRateAsync(Agent agent, decimal monthlySalesIncludingCurrentBooking);
        
        Task<decimal> CalculateCommissionAsync(Agent agent, decimal ticketAmount, decimal monthlySalesIncludingCurrentBooking);
    }
}
