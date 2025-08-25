using BookingHotel.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models.User
{
    public class UpdateUserDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(15)]
        public string? Phone { get; set; }

    }
}
