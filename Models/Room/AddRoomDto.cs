using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models.Room
{
    public class AddRoomDto
    {
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
