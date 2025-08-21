using BookingHotel.Entities;
using BookingHotel.Models.RoomCategory;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace BookingHotel.Models.Room
{
    public class RoomWithoutCategoryDto
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string RoomName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int FloorNumber { get; set; }

        public bool IsAvailable { get; set; }

        [Required]
        public Guid RoomCategoryId { get; set; }
    }
}
