using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public Guid RoomId { get; set; }
        public Room? Room { get; set; }

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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public enum BookingStatus
    {
        Pending,
        CheckedIn,
        CheckedOut,
        Canceled   
    }
}
