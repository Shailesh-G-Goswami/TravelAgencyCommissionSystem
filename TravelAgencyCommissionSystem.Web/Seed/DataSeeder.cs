using Microsoft.EntityFrameworkCore;
using TravelAgencyCommissionSystem.Web.Data;
using TravelAgencyCommissionSystem.Web.Models;
using TravelAgencyCommissionSystem.Web.Models.Enums;

namespace TravelAgencyCommissionSystem.Web.Seed
{
    public class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await SeedCommissionTiersAsync(context);
            await context.SaveChangesAsync();

            await SeedAgentsAsync(context);
            await context.SaveChangesAsync();

            await SeedBookingsAsync(context);
            await context.SaveChangesAsync();
        }

        private static async Task SeedCommissionTiersAsync(ApplicationDbContext context)
        {
            if(await context.CommissionTiers.AnyAsync())
            {
                return;
            }

            var tiers = new List<CommissionTier>
            {
                new()
                {
                    MinMonthlySales = 0,
                    MaxMonthlySales = 50000,
                    RatePercent = 2
                },
                new()
                {
                    MinMonthlySales = 50001,
                    MaxMonthlySales = 200000,
                    RatePercent = 3.5m
                },
                new()
                {
                    MinMonthlySales = 200001,
                    MaxMonthlySales = null,
                    RatePercent = 5
                }
            };

            await context.CommissionTiers.AddRangeAsync(tiers);
        }

        private static async Task SeedAgentsAsync(ApplicationDbContext context)
        {
            if (await context.Agents.AnyAsync())
            {
                return;
            }

            var agents = new List<Agent>
            {
                new() { Name = "John Smith" },
                new() { Name = "Sarah Johnson" },
                new() { Name = "Michael Brown" },
                new() { Name = "Emma Wilson", TierOverrideRate = 4 },
                new() { Name = "David Lee", TierOverrideRate = 0 }
            };

            await context.Agents.AddRangeAsync(agents);
        }

        private static async Task SeedBookingsAsync(ApplicationDbContext context)
        {
            if (await context.Bookings.AnyAsync())
            {
                return;
            }

            var agents = await context.Agents.ToListAsync();

            var john = agents.First(a => a.Name == "John Smith") 
                ?? throw new Exception("John Smith not found in seed data.");
            var sarah = agents.First(a => a.Name == "Sarah Johnson") 
                ?? throw new Exception("Sarah Johnson not found in seed data.");
            var michael = agents.First(a => a.Name == "Michael Brown") 
                ?? throw new Exception("Michael Brown not found in seed data.");
            var emma = agents.First(a => a.Name == "Emma Wilson") 
                ?? throw new Exception("Emma Wilson not found in seed data.");
            var david = agents.First(a => a.Name == "David Lee") 
                ?? throw new Exception("David Lee not found in seed data.");

            var bookings = new List<Booking>
    {
        // John → exactly 50,000
        new()
        {
            AgentId = john.Id,
            TicketAmount = 30000,
            BookingDate = new DateTime(2026, 7, 5, 0, 0, 0, DateTimeKind.Utc),
            Status = BookingStatus.Confirmed
        },
        new()
        {
            AgentId = john.Id,
            TicketAmount = 20000,
            BookingDate = new DateTime(2026, 7, 10, 0, 0, 0, DateTimeKind.Utc),
            Status = BookingStatus.Confirmed
        },

        // Sarah → exactly 50,001
        new()
        {
            AgentId = sarah.Id,
            TicketAmount = 50001,
            BookingDate = new DateTime(2026, 7, 7, 0, 0, 0, DateTimeKind.Utc),
            Status = BookingStatus.Confirmed
        },

        // Michael → 200,001
        new()
        {
            AgentId = michael.Id,
            TicketAmount = 100000,
            BookingDate = new DateTime(2026, 7, 3, 0, 0, 0, DateTimeKind.Utc),
            Status = BookingStatus.Confirmed
        },
        new()
        {
            AgentId = michael.Id,
            TicketAmount = 100001,
            BookingDate = new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc),
            Status = BookingStatus.Confirmed
        },

        // Emma → Override 4%
        new()
        {
            AgentId = emma.Id,
            TicketAmount = 80000,
            BookingDate = new DateTime(2026, 7, 8, 0, 0, 0, DateTimeKind.Utc),
            Status = BookingStatus.Confirmed
        },

        // David → Override 0%
        new()
        {
            AgentId = david.Id,
            TicketAmount = 60000,
            BookingDate = new DateTime(2026, 7, 12, 0, 0, 0, DateTimeKind.Utc),
            Status = BookingStatus.Confirmed
        },

        // Cancelled booking
        new()
        {
            AgentId = john.Id,
            TicketAmount = 15000,
            BookingDate = new DateTime(2026, 7, 20, 0, 0, 0, DateTimeKind.Utc),
            Status = BookingStatus.Cancelled
        },

        // Pending booking
        new()
        {
            AgentId = sarah.Id,
            TicketAmount = 25000,
            BookingDate = new DateTime(2026, 7, 22, 0, 0, 0, DateTimeKind.Utc),
            Status = BookingStatus.Pending
        },

        // Previous month booking
        new()
        {
            AgentId = john.Id,
            TicketAmount = 10000,
            BookingDate = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc),
            Status = BookingStatus.Confirmed
        }
    };

            await context.Bookings.AddRangeAsync(bookings);
        }
    }
}
