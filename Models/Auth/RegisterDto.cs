using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models.Auth
{
    public class RegisterDto
    {
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
        public required string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password do not match.")]
        public required string ConfirmPassword { get; set; }
    }
}
