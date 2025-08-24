using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookingHotel.Data;
using BookingHotel.Entities;
using BookingHotel.Models.Booking;
using BookingHotel.Models.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingHotel.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BookingController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBookings(
            [FromQuery] int limit = 10,
            [FromQuery] int page = 1,
            [FromQuery] string roomName = ""
            )
        {
            if (limit <= 0) limit = 10;
            if (page <= 0) page = 1;

            IQueryable<Booking> query = _context.Bookings.Include(b => b.Room);

            if (roomName != "")
            {
                query = query.Where(r => r.Room != null && r.Room.RoomName.Contains(roomName));
            }

            var counts = await query.CountAsync();
            var totalPage = Math.Ceiling(counts / (double) limit);

            query = query.Skip((page - 1) * limit).Take(limit);

            var data = await query.ProjectTo<BookingDto>(_mapper.ConfigurationProvider).ToListAsync();
            return Ok(new { limit, page, totalPage, counts, data });
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetBookingById(Guid id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var data = await _context.Bookings.ProjectTo<BookingDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

            if (data is null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateBooking([FromBody] AddBookingDto addBookingDto)
        {
            if (addBookingDto.CheckOutDate <= addBookingDto.CheckInDate || addBookingDto.CheckInDate < DateTime.Today)
            {
                return BadRequest(new { message = "Check-out date must be later than check-in date and must be later than today." });
            }

            var isRoomBooked = await _context.Bookings.AnyAsync(b => 
                b.RoomId == addBookingDto.RoomId &&  
                (b.CheckInDate >= addBookingDto.CheckInDate &&
                b.CheckOutDate <= addBookingDto.CheckOutDate) || 
                (addBookingDto.CheckInDate >= b.CheckInDate &&
                addBookingDto.CheckOutDate <= b.CheckOutDate)
            );

            if (isRoomBooked)
            {
                return BadRequest(new { message = "Room is already booked for the selected date." });
            }

            var room = await _context.Rooms.ProjectTo<RoomDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(r => r.Id == addBookingDto.RoomId);

            if (room is null || room.RoomCategory is null)
            {
                return BadRequest(new { message = "Room not found." });
            }

            if (addBookingDto.NumberOfGuests > room.RoomCategory.Capacity)
            {
                return BadRequest(new { message = "The number of guests exceeds the maximum capacity of the room." }); 
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            int nights = (int) (addBookingDto.CheckOutDate - addBookingDto.CheckInDate).TotalDays;
            int totalPrice = nights * room.RoomCategory.PricePerNight;

            var bookingData = _mapper.Map<Booking>(addBookingDto);

            bookingData.UserId = userId;
            bookingData.TotalPrice = totalPrice;

            _context.Bookings.Add(bookingData);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetBookingById), new {id = bookingData.Id}, _mapper.Map<BookingDto>(bookingData));
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] UpdateBookingDto updateBookingDto)
        {
            var bookingData = await _context.Bookings.Include(r => r.Room).ThenInclude(rc => rc.RoomCategory).FirstOrDefaultAsync(b => b.Id == id);

            if (bookingData is null)
            {
                return NotFound();
            }

            if (updateBookingDto.CheckOutDate <= updateBookingDto.CheckInDate || updateBookingDto.CheckInDate < DateTime.Today)
            {
                return BadRequest(new { message = "Check-out date must be later than check-in date and must be later than today." });
            }

            if (updateBookingDto.RoomId is null)
            {
                updateBookingDto.RoomId = bookingData.RoomId;
            }
            
            var isRoomBooked = await _context.Bookings.AnyAsync(b =>
                b.RoomId == updateBookingDto.RoomId &&
                (b.CheckInDate >= updateBookingDto.CheckInDate &&
                b.CheckOutDate <= updateBookingDto.CheckOutDate) ||
                (updateBookingDto.CheckInDate >= b.CheckInDate &&
                updateBookingDto.CheckOutDate <= b.CheckOutDate)
            );

            if (isRoomBooked)
            {
                return BadRequest(new { message = "Room is already booked for the selected date." });
            }

            var room = await _context.Rooms.Include(r => r.RoomCategory).FirstOrDefaultAsync(r => r.Id == updateBookingDto.RoomId);

            if (updateBookingDto.NumberOfGuests > room.RoomCategory.Capacity)
            {
                return BadRequest(new { message = "The number of guests exceeds the maximum capacity of the room." });
            }

            int nights = (int)(updateBookingDto.CheckOutDate - updateBookingDto.CheckInDate).TotalDays;
            int totalPrice = nights * room.RoomCategory.PricePerNight;
                
            _mapper.Map(updateBookingDto, bookingData);
            bookingData.TotalPrice = totalPrice;
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<BookingDto>(bookingData));
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> RemoveBooking(Guid id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var data = await _context.Bookings.FirstOrDefaultAsync(b => b.UserId == userId && b.Id == id);

            if (data is null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(data);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
