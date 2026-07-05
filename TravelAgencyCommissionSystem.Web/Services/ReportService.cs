using Microsoft.EntityFrameworkCore;
using TravelAgencyCommissionSystem.Web.Data;
using TravelAgencyCommissionSystem.Web.DTOs;
using TravelAgencyCommissionSystem.Web.Models.Enums;
using TravelAgencyCommissionSystem.Web.Services.Interfaces;

namespace TravelAgencyCommissionSystem.Web.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<CommissionSummaryDto>>
            GetCommissionSummaryAsync(
                string month,
                int page,
                int pageSize,
                string sortBy,
                bool descending)
        {
            if (!DateTime.TryParseExact(
                month,
                "yyyy-MM",
                null,
                System.Globalization.DateTimeStyles.None,
                out var monthDate))
            {
                throw new ArgumentException("Month must be in yyyy-MM format.");
            }

            var startDate =
                new DateTime(
                    monthDate.Year,
                    monthDate.Month,
                    1,
                    0,
                    0,
                    0,
                    DateTimeKind.Utc);

            var endDate = startDate.AddMonths(1);

            //var query =
            //    _context.Agents
            //        .Select(agent => new CommissionSummaryDto
            //        {
            //            AgentId = agent.Id,
            //            AgentName = agent.Name,

            //            TotalConfirmedSales =
            //                agent.Bookings
            //                    .Where(b =>
            //                        b.Status == BookingStatus.Confirmed &&
            //                        b.BookingDate >= startDate &&
            //                        b.BookingDate < endDate)
            //                    .Sum(b => (decimal?)b.TicketAmount) ?? 0,

            //            TotalReservedCommission =
            //                agent.Bookings
            //                    .Where(b =>
            //                        b.BookingDate >= startDate &&
            //                        b.BookingDate < endDate &&
            //                        b.CommissionReservation != null &&
            //                        !b.CommissionReservation.IsVoided)
            //                    .Sum(b => (decimal?)b.CommissionReservation!.ReservedAmount) ?? 0,

            //            MostFrequentTier = null
            //        });

            var query =
                _context.Agents
                    .Where(agent =>
                        agent.Bookings.Any(b =>
                            b.BookingDate >= startDate &&
                            b.BookingDate < endDate))
                    .Select(agent => new CommissionSummaryDto
                    {
                        AgentId = agent.Id,
                        AgentName = agent.Name,

                        TotalConfirmedSales =
                            agent.Bookings
                                .Where(b =>
                                    b.Status == BookingStatus.Confirmed &&
                                    b.BookingDate >= startDate &&
                                    b.BookingDate < endDate)
                                .Sum(b => (decimal?)b.TicketAmount) ?? 0,

                        TotalReservedCommission =
                            agent.Bookings
                                .Where(b =>
                                    b.BookingDate >= startDate &&
                                    b.BookingDate < endDate &&
                                    b.CommissionReservation != null &&
                                    !b.CommissionReservation.IsVoided)
                                .Sum(b =>
                                    (decimal?)b.CommissionReservation!.ReservedAmount) ?? 0,

                        MostFrequentTier = "N/A"
                    });

            query = sortBy?.ToLower() switch
            {
                "sales" => descending
                    ? query.OrderByDescending(x => x.TotalConfirmedSales)
                    : query.OrderBy(x => x.TotalConfirmedSales),

                "commission" => descending
                    ? query.OrderByDescending(x => x.TotalReservedCommission)
                    : query.OrderBy(x => x.TotalReservedCommission),

                _ => descending
                    ? query.OrderByDescending(x => x.AgentName)
                    : query.OrderBy(x => x.AgentName)
            };

            var totalCount = await query.CountAsync();

            var items =
                await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            foreach (var item in items)
            {
                item.MostFrequentTier =
                    await GetMostFrequentTierAsync(
                        item.AgentId,
                        startDate,
                        endDate);
            }

            return new PagedResultDto<CommissionSummaryDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }


        private async Task<string> GetMostFrequentTierAsync(
            int agentId,
            DateTime startDate,
            DateTime endDate)
        {
            var result = await _context.CommissionReservations
                .Where(r =>
                    !r.IsVoided &&
                    r.Booking.AgentId == agentId &&
                    r.Booking.BookingDate >= startDate &&
                    r.Booking.BookingDate < endDate)
                .GroupBy(r => r.RateApplied)
                .Select(g => new
                {
                    Rate = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Rate)   // tie-break rule: lower rate wins
                .FirstOrDefaultAsync();

            if (result == null)
            {
                return "N/A";
            }

            return $"{result.Rate}%";
        }
    }
}
