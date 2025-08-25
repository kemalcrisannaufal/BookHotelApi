using BCrypt.Net;
using BookingHotel.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ApplicationDbContext()
        {
        }        

        public DbSet<User> Users { get; set; } 
        public DbSet<RoomCategory> RoomCategories { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>() 
                .HasColumnType("nvarchar(10)");

            modelBuilder.Entity<Booking>()
                .Property(u => u.Status)
                .HasConversion<string>()
                .HasColumnType("nvarchar(20)");

            modelBuilder.Entity<Room>().
                HasOne(r => r.RoomCategory).
                WithMany(rc => rc.Rooms).
                HasForeignKey(r => r.RoomCategoryId).
                OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>().
                HasOne(b => b.User).
                WithMany(u => u.Bookings).
                HasForeignKey(b => b.UserId).
                OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>().
                HasOne(b => b.Room).
                WithMany(r => r.Bookings).
                HasForeignKey(b => b.RoomId).
                OnDelete(DeleteBehavior.Cascade);

        }
    }
}
