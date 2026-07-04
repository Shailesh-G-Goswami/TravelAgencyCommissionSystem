using Microsoft.EntityFrameworkCore;
using TravelAgencyCommissionSystem.Web.Models;

namespace TravelAgencyCommissionSystem.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Agent> Agents => Set<Agent>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<CommissionReservation> CommissionReservations => Set<CommissionReservation>();
        public DbSet<CommissionTier> CommissionTiers => Set<CommissionTier>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            ConfigureAgent(modelBuilder);
            ConfigureBooking(modelBuilder);
            ConfigureCommissionReservation(modelBuilder);
            ConfigureCommissionTier(modelBuilder);

        }

        private static void ConfigureAgent(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>(entity =>
            {
                entity.Property(x => x.Name)
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(x => x.TierOverrideRate)
                      .HasPrecision(5, 2);
            });
        }

        private static void ConfigureBooking(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(x => x.TicketAmount)
                      .HasPrecision(18, 2);

                entity.HasOne(x => x.Agent)
                      .WithMany(x => x.Bookings)
                      .HasForeignKey(x => x.AgentId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Performance indexes
                entity.HasIndex(x => x.AgentId);
                entity.HasIndex(x => x.BookingDate);
                entity.HasIndex(x => x.Status);

                // Useful composite index for monthly sales queries
                entity.HasIndex(x => new
                {
                    x.AgentId,
                    x.BookingDate,
                    x.Status
                });
            });
        }

        private static void ConfigureCommissionReservation(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommissionReservation>(entity =>
            {
                entity.Property(x => x.ReservedAmount)
                      .HasPrecision(18, 2);

                entity.Property(x => x.RateApplied)
                      .HasPrecision(5, 2);

                entity.Property(x => x.RowVersion)
                      .IsRowVersion();

                entity.HasOne(x => x.Booking)
                      .WithOne(x => x.CommissionReservation)
                      .HasForeignKey<CommissionReservation>(x => x.BookingId)
                      .OnDelete(DeleteBehavior.Restrict);

                // One reservation per booking
                entity.HasIndex(x => x.BookingId)
                      .IsUnique();
            });
        }

        private static void ConfigureCommissionTier(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommissionTier>(entity =>
            {
                entity.Property(x => x.MinMonthlySales)
                      .HasPrecision(18, 2);

                entity.Property(x => x.MaxMonthlySales)
                      .HasPrecision(18, 2);

                entity.Property(x => x.RatePercent)
                      .HasPrecision(5, 2);
            });
        }
    }
}
