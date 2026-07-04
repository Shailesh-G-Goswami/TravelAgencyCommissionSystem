using Microsoft.EntityFrameworkCore;
using System.Data;
using TravelAgencyCommissionSystem.Web.Data;
using TravelAgencyCommissionSystem.Web.DTOs;
using TravelAgencyCommissionSystem.Web.Exceptions;
using TravelAgencyCommissionSystem.Web.Models;
using TravelAgencyCommissionSystem.Web.Models.Enums;
using TravelAgencyCommissionSystem.Web.Services.Interfaces;

namespace TravelAgencyCommissionSystem.Web.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICommissionService _commissionService;

        public BookingService(
            ApplicationDbContext context,
            ICommissionService commissionService)
        {
            _context = context;
            _commissionService = commissionService;
        }

        public async Task<CommissionReservation> ReserveCommissionAsync(int bookingId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            var booking = await _context.Bookings
                .Include(x => x.Agent)
                .Include(x => x.CommissionReservation)
                .FirstOrDefaultAsync(x => x.Id == bookingId);

            if (booking == null)
            {
                throw new NotFoundException("Booking not found.");
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                throw new BusinessRuleException(
                    "Cannot reserve commission for a cancelled booking.");
            }

            if (booking.Status != BookingStatus.Confirmed)
            {
                throw new BusinessRuleException(
                    "Commission can only be reserved for confirmed bookings.");
            }

            if (booking.CommissionReservation != null &&
                !booking.CommissionReservation.IsVoided)
            {
                throw new BusinessRuleException(
                    "Commission has already been reserved for this booking.");
            }

            var year = booking.BookingDate.Year;
            var month = booking.BookingDate.Month;

            var monthlySales =
                await GetMonthlyConfirmedSalesAsync(
                    booking.AgentId,
                    year,
                    month);

            // Include current booking if it is confirmed
            if (booking.Status == BookingStatus.Confirmed)
            {
                monthlySales += booking.TicketAmount;
            }

            var rate =
                await _commissionService.GetApplicableRateAsync(
                    booking.Agent,
                    monthlySales);

            var commission =
                await _commissionService.CalculateCommissionAsync(
                    booking.Agent,
                    booking.TicketAmount,
                    monthlySales);

            var reservation = new CommissionReservation
            {
                BookingId = booking.Id,
                ReservedAmount = commission,
                RateApplied = rate,
                CreatedAt = DateTime.UtcNow,
                IsVoided = false
            };

            _context.CommissionReservations.Add(reservation);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return reservation;
        }

        public async Task CancelBookingAsync(int bookingId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            var booking = await _context.Bookings
                .Include(x => x.CommissionReservation)
                .FirstOrDefaultAsync(x => x.Id == bookingId);

            if (booking == null)
            {
                throw new NotFoundException("Booking not found.");
            }

            if (booking.Status == BookingStatus.Cancelled)
            {
                throw new BusinessRuleException(
                    "Booking has already been cancelled.");
            }

            booking.Status = BookingStatus.Cancelled;

            if (booking.CommissionReservation != null)
            {
                booking.CommissionReservation.IsVoided = true;
                booking.CommissionReservation.VoidedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public async Task<decimal> GetMonthlyConfirmedSalesAsync(
            int agentId,
            int year,
            int month)
        {
            var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endDate = startDate.AddMonths(1);

            return await _context.Bookings
                .Where(x =>
                    x.AgentId == agentId &&
                    x.Status == BookingStatus.Confirmed &&
                    x.BookingDate == startDate &&
                    x.BookingDate == endDate)
                .SumAsync(x => (decimal?)x.TicketAmount) ?? 0m;
        }

        public async Task<BookingDetailsDto> GetBookingDetailsAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(x => x.Agent)
                .Include(x => x.CommissionReservation)
                .FirstOrDefaultAsync(x => x.Id == bookingId);

            if (booking == null)
            {
                throw new NotFoundException("Booking not found.");
            }

            return new BookingDetailsDto
            {
                Id = booking.Id,
                AgentName = booking.Agent.Name,
                TicketAmount = booking.TicketAmount,
                Status = booking.Status.ToString(),
                BookingDate = booking.BookingDate,

                ReservedAmount =
                    booking.CommissionReservation?.IsVoided == false
                        ? booking.CommissionReservation.ReservedAmount
                        : null,

                RateApplied =
                    booking.CommissionReservation?.IsVoided == false
                        ? booking.CommissionReservation.RateApplied
                        : null,

                HasActiveReservation =
                    booking.CommissionReservation != null &&
                    !booking.CommissionReservation.IsVoided
            };
        }
    }
}
