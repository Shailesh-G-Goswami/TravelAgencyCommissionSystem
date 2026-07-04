using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelAgencyCommissionSystem.Web.Data;
using TravelAgencyCommissionSystem.Web.Models;
using TravelAgencyCommissionSystem.Web.Services;

namespace TravelAgencyCommissionSystem.Tests.Services
{
    public class CommissionServiceTests
    {
        private async Task<ApplicationDbContext> CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);

            context.CommissionTiers.AddRange(
                new CommissionTier
                {
                    MinMonthlySales = 0,
                    MaxMonthlySales = 50000,
                    RatePercent = 2
                },
                new CommissionTier
                {
                    MinMonthlySales = 50001,
                    MaxMonthlySales = 200000,
                    RatePercent = 3.5m
                },
                new CommissionTier
                {
                    MinMonthlySales = 200001,
                    MaxMonthlySales = null,
                    RatePercent = 5
                });

            await context.SaveChangesAsync();

            return context;
        }

        [Fact]
        public async Task Exactly_50000_Should_Use_2_Percent()
        {
            var context = await CreateContext();
            var service = new CommissionService(context);

            var agent = new Agent { Name = "John" };

            var rate = await service.GetApplicableRateAsync(agent, 50000);

            Assert.Equal(2m, rate);
        }

        [Fact]
        public async Task Exactly_50001_Should_Use_3_5_Percent()
        {
            var context = await CreateContext();
            var service = new CommissionService(context);

            var agent = new Agent();

            var rate = await service.GetApplicableRateAsync(agent, 50001);

            Assert.Equal(3.5m, rate);
        }

        [Fact]
        public async Task Exactly_200000_Should_Use_3_5_Percent()
        {
            var context = await CreateContext();
            var service = new CommissionService(context);

            var agent = new Agent();

            var rate = await service.GetApplicableRateAsync(agent, 200000);

            Assert.Equal(3.5m, rate);
        }

        [Fact]
        public async Task Exactly_200001_Should_Use_5_Percent()
        {
            var context = await CreateContext();
            var service = new CommissionService(context);

            var agent = new Agent();

            var rate = await service.GetApplicableRateAsync(agent, 200001);

            Assert.Equal(5m, rate);
        }

        [Fact]
        public async Task TierOverrideRate_Zero_Should_Use_Zero()
        {
            var context = await CreateContext();
            var service = new CommissionService(context);

            var agent = new Agent
            {
                TierOverrideRate = 0m
            };

            var rate = await service.GetApplicableRateAsync(agent, 999999);

            Assert.Equal(0m, rate);
        }
    }
}
