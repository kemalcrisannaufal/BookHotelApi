using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models.Auth
{
    public class UpdatePasswordDto
    {
        [Required]
        public required string CurrentPassword { get; set; }

        [Required]
        public required string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public required string ConfirmNewPassword { get; set; }

    }
}
