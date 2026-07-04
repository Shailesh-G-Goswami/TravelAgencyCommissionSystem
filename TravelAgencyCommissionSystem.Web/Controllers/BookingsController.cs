using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravelAgencyCommissionSystem.Web.DTOs;
using TravelAgencyCommissionSystem.Web.Exceptions;
using TravelAgencyCommissionSystem.Web.Services.Interfaces;

namespace TravelAgencyCommissionSystem.Web.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("{bookingId:int}/reserve-commission")]
        public async Task<IActionResult> ReserveCommission(int bookingId)
        {
            try
            {
                var reservation =
                    await _bookingService.ReserveCommissionAsync(bookingId);

                var response = new CommissionReservationResponseDto
                {
                    ReservationId = reservation.Id,
                    BookingId = reservation.BookingId,
                    ReservedAmount = reservation.ReservedAmount,
                    RateApplied = reservation.RateApplied,
                    CreatedAt = reservation.CreatedAt
                };

                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiErrorDto
                {
                    Message = ex.Message
                });
            }
            catch (BusinessRuleException ex)
            {
                return Conflict(new ApiErrorDto
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{bookingId:int}/cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            try
            {
                await _bookingService.CancelBookingAsync(bookingId);

                return Ok(new
                {
                    Message = "Booking cancelled successfully."
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiErrorDto
                {
                    Message = ex.Message
                });
            }
            catch (BusinessRuleException ex)
            {
                return Conflict(new ApiErrorDto
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{bookingId:int}")]
        public async Task<IActionResult> GetBooking(int bookingId)
        {
            try
            {
                var booking =
                    await _bookingService.GetBookingDetailsAsync(bookingId);

                return Ok(booking);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiErrorDto
                {
                    Message = ex.Message
                });
            }
        }
    }
}
