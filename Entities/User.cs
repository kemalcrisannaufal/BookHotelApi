using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingHotel.Entities
{

    public class User
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(15)]
        public required string Phone { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [Column(TypeName = " nvarchar(10)")]
        [EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; } = UserRole.User;

        [Required]
        public required string PasswordHash { get; set; }
        
        public ICollection<Booking>? Bookings = new List<Booking>(); 
    }

    public enum UserRole
    {
        User,
        Admin,
        SuperAdmin,
    }
}
