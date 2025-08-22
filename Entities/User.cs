using System.ComponentModel.DataAnnotations;

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
        public string Role { get; set; } = "User";

        [Required]
        public required string PasswordHash { get; set; }
        
    }
}
