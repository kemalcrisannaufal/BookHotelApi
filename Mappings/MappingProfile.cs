using AutoMapper;
using BookingHotel.Entities;
using BookingHotel.Models.Auth;
using BookingHotel.Models.Booking;
using BookingHotel.Models.Room;
using BookingHotel.Models.RoomCategory;
using BookingHotel.Models.User;

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

            //User Mapping
            CreateMap<User, UserDto>();
            CreateMap<RegisterDto, User>().ForMember(
                dest => dest.PasswordHash,
                opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password))
                );
            CreateMap<UpdatePasswordDto, User>().ForMember(
                dest => dest.PasswordHash,
                opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.NewPassword))
                ).ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            //Booking Mapping
            CreateMap<Booking, BookingDto>();
            CreateMap<AddBookingDto, Booking>().
                ForMember(
                dest => dest.Status, 
                opt => opt.MapFrom(src => BookingStatus.Pending));
            CreateMap<UpdateBookingDto, Booking>().ForMember(
                dest => dest.RoomId,
                opt => opt.Condition(src => src.RoomId != Guid.Empty)).ForMember(
                dest => dest.UpdatedAt,
                opt => opt.MapFrom(src => DateTime.UtcNow)
                ).
                ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            
        }
    }
}
