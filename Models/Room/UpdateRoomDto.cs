using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models.Room
{
    public class UpdateRoomDto
    {
        [MaxLength(50)]
        public string? RoomName { get; set; }

        [Range(1, int.MaxValue)]
        public int? FloorNumber { get; set; }

        public bool? IsAvailable { get; set; }

        public Guid? RoomCategoryId { get; set; } = Guid.Empty;
    }
}
