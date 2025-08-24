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
