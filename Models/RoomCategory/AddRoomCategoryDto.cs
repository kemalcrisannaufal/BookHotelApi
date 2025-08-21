using System.ComponentModel.DataAnnotations;

namespace BookingHotel.Models.RoomCategory
{
    public class AddRoomCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public required string Description { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Bed { get; set; }

        [Required]
        [MaxLength(500)]
        public required string Amenities { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Capacity { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int PricePerNight { get; set; }

        public string? Image { get; set; }
    }
}
