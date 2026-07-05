using TravelAgencyCommissionSystem.Web.DTOs;
using TravelAgencyCommissionSystem.Web.Models;

namespace TravelAgencyCommissionSystem.Web.Services.Interfaces
{
    public interface IBookingService
    {
        Task<CommissionReservation> ReserveCommissionAsync(int bookingId);

        Task CancelBookingAsync(int bookingId);

        Task<decimal> GetMonthlyConfirmedSalesAsync(int agentId,DateTime bookingDate);

        Task<BookingDetailsDto> GetBookingDetailsAsync(int bookingId);

    }
}
