using BookingHotel.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models.Booking
{
    public class UpdateBookingDto
    {
        public Guid? RoomId { get; set; }

        public required DateTime CheckInDate { get; set; }

        public required DateTime CheckOutDate { get; set; }

        [Range(1, int.MaxValue)]
        public int? NumberOfGuests { get; set; }

        public BookingStatus? Status { get; set; }
    }
}
