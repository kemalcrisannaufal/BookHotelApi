using BookingHotel.Entities;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BookingHotel.Models.User
{
    public class UpdateRoleDto
    {
        [Required]
        public required string Role { get; set; }
    }
}
