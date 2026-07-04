using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgencyCommissionSystem.Web.Data;
using TravelAgencyCommissionSystem.Web.Exceptions;
using TravelAgencyCommissionSystem.Web.Models;
using TravelAgencyCommissionSystem.Web.Models.Enums;
using TravelAgencyCommissionSystem.Web.Services;

namespace TravelAgencyCommissionSystem.Tests.Services
{
    public class BookingServiceTests
    {
        [Fact]
        public async Task Cannot_Reserve_Cancelled_Booking()
        {
            var options =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .ConfigureWarnings(w=>
                        w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                    .Options;

            await using var context =
                new ApplicationDbContext(options);

            var agent = new Agent
            {
                Name = "John"
            };

            context.Agents.Add(agent);

            await context.SaveChangesAsync();

            var booking = new Booking
            {
                AgentId = agent.Id,
                TicketAmount = 10000,
                BookingDate = DateTime.UtcNow,
                Status = BookingStatus.Cancelled
            };

            context.Bookings.Add(booking);

            await context.SaveChangesAsync();

            var commissionService =
                new CommissionService(context);

            var bookingService =
                new BookingService(
                    context,
                    commissionService);

            await Assert.ThrowsAsync<BusinessRuleException>(
                () => bookingService.ReserveCommissionAsync(
                    booking.Id));
        }
    }
}
