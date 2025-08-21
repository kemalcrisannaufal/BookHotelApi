using AutoMapper;
using BookingHotel.Entities;
using BookingHotel.Models.Room;
using BookingHotel.Models.RoomCategory;

namespace BookingHotel.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            //Room Category Mapping
            CreateMap<RoomCategory, RoomCategoryDto>();
            CreateMap<AddRoomCategoryDto, RoomCategory>();
            CreateMap<UpdateRoomCategoryDto, RoomCategory>().
                ForAllMembers(
                opt => opt.Condition((src, dest, srcMember) => srcMember != null)
                );

            //Room Mapping
            CreateMap<Room, RoomDto>();
            CreateMap<Room, RoomWithoutCategoryDto>();
            CreateMap<AddRoomDto, Room>();
            CreateMap<UpdateRoomDto, Room>()
               .ForMember(dest => dest.RoomCategoryId,
               opt => opt.Condition(src => src.RoomCategoryId != Guid.Empty))
               .ForAllMembers(opt =>
               opt.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
