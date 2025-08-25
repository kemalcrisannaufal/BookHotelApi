using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookingHotel.Data;
using BookingHotel.Entities;
using BookingHotel.Models.ApplicationResponse;
using BookingHotel.Models.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RoomController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms(
            [FromQuery] int limit = 10,
            [FromQuery] int page = 1,
            [FromQuery] string search = ""
            )
        {
            if (limit <= 0) limit = 10;
            if (page <= 0) page = 1;
            
            IQueryable<Room> query = _context.Rooms;

            if (search != "")
            {
                query = query.Where(r => r.RoomName.Contains(search));
            }

            var total = await query.CountAsync();
            var totalPages = (int) Math.Ceiling(total / (double)limit);

            query = query.Skip((page-1)*limit).Take(limit);
            var data = await query.ProjectTo<RoomDto>(_mapper.ConfigurationProvider).ToListAsync();

            var response = new PaginationResponse<RoomDto>(limit, page, total, totalPages, data);
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetRoomById(Guid id)
        {
            var room = await _context.Rooms.ProjectTo<RoomDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(r => r.Id == id);

            if (room is null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRoom([FromBody] AddRoomDto addRoomDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isValidRoomCategory = await _context.RoomCategories.AnyAsync(c => c.Id == addRoomDto.RoomCategoryId);

            if (!isValidRoomCategory) 
            {
                return NotFound(new { message = "Room category not found."});
            }

            var room = _mapper.Map<Room>(addRoomDto);
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoomById), new {id = room.Id}, _mapper.Map<RoomDto>(room));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] UpdateRoomDto updateRoomDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var room = await _context.Rooms.FindAsync(id);

            if (room is null) 
            {
                return NotFound();
            }

            var isValidRoomCategory = true;

            if (updateRoomDto.RoomCategoryId != Guid.Empty)
            {
                isValidRoomCategory = await _context.RoomCategories.AnyAsync(rc => rc.Id == updateRoomDto.RoomCategoryId);
            }
                
            if (!isValidRoomCategory)
            {
                return NotFound(new { message = "Room category not found." });
            }

            _mapper.Map(updateRoomDto, room);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<RoomDto>(room));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRoom(Guid id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room is null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();

        }

    }
}
