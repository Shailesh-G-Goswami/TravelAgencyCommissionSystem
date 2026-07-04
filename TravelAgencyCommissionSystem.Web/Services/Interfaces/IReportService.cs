using TravelAgencyCommissionSystem.Web.DTOs;

namespace TravelAgencyCommissionSystem.Web.Services.Interfaces
{
    public interface IReportService
    {
        Task<PagedResultDto<CommissionSummaryDto>>
            GetCommissionSummaryAsync(
                string month,
                int page,
                int pageSize,
                string sortBy,
                bool descending);
    }
}
