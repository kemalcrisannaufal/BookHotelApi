using BookingHotel.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models.Booking
{
    public class AddBookingDto
    {
        [Required]
        public Guid RoomId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int NumberOfGuests { get; set; }

    }
}
