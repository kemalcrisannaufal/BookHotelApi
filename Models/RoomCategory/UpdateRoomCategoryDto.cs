using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models.RoomCategory
{
    public class UpdateRoomCategoryDto
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        
        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Bed { get; set; }

        [MaxLength(500)]
        public string? Amenities { get; set; }

        [Range(0, int.MaxValue)]
        public int? Capacity { get; set; }

        [Range(0, int.MaxValue)]
        public int? PricePerNight { get; set; }

        public string? Image { get; set; }
    }
}
