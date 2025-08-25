using BookingHotel.Entities;
using BookingHotel.Models.Room;
using BookingHotel.Models.User;
using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models.Booking
{
    public class BookingDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public Guid RoomId { get; set; }
        public RoomDto? Room { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }
        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int NumberOfGuests { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TotalPrice { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}
