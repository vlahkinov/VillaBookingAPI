using Microsoft.EntityFrameworkCore;
using VillaBookingAPI.Models;

namespace VillaBookingAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Booking entity
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.Property(b => b.ClientName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(b => b.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(100);

                // Index for fast overlap queries per house
                entity.HasIndex(b => new { b.HouseId, b.StartDate, b.EndDate })
                    .HasDatabaseName("IX_Bookings_HouseId_Dates");
            });

            // Seed sample data
            modelBuilder.Entity<Booking>().HasData(
                new Booking
                {
                    Id = 1,
                    ClientName = "Иван Петров",
                    GuestsCount = 2,
                    StartDate = new DateTime(2025, 7, 10),
                    EndDate = new DateTime(2025, 7, 15),
                    HouseId = 1,
                    IsDepositPaid = true,
                    CreatedBy = "admin"
                },
                new Booking
                {
                    Id = 2,
                    ClientName = "Мария Иванова",
                    GuestsCount = 4,
                    StartDate = new DateTime(2025, 7, 20),
                    EndDate = new DateTime(2025, 7, 25),
                    HouseId = 1,
                    IsDepositPaid = false,
                    CreatedBy = "admin"
                },
                new Booking
                {
                    Id = 3,
                    ClientName = "Георги Димитров",
                    GuestsCount = 3,
                    StartDate = new DateTime(2025, 7, 10),
                    EndDate = new DateTime(2025, 7, 18),
                    HouseId = 2,
                    IsDepositPaid = true,
                    CreatedBy = "admin"
                },
                new Booking
                {
                    Id = 4,
                    ClientName = "Елена Стоянова",
                    GuestsCount = 1,
                    StartDate = new DateTime(2025, 8, 1),
                    EndDate = new DateTime(2025, 8, 5),
                    HouseId = 2,
                    IsDepositPaid = true,
                    CreatedBy = "mobile_user"
                },
                new Booking
                {
                    Id = 5,
                    ClientName = "Петър Николов",
                    GuestsCount = 2,
                    StartDate = new DateTime(2025, 8, 10),
                    EndDate = new DateTime(2025, 8, 14),
                    HouseId = 1,
                    IsDepositPaid = false,
                    CreatedBy = "mobile_user"
                }
            );
        }
    }
}
