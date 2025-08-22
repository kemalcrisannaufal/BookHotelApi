using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookingHotel.Data;
using BookingHotel.Entities;
using BookingHotel.Models.Room;
using BookingHotel.Models.RoomCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Controllers
{
    [Route("api/room-categories")]
    [ApiController]
    public class RoomCategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RoomCategoryController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoomCategories(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10,
            [FromQuery] string search = ""
            )
        {
            if (page <= 0) page = 1;
            if (limit <= 0) limit = 10;

            IQueryable<RoomCategory> query = _context.RoomCategories;

            if (search != "")
            {
                query = query.Where(rc => rc.Name.Contains(search));
            }

            var total = await query.CountAsync();
            var totalPages = Math.Ceiling(total / (double)limit);

            query = query.Skip((page - 1) * limit).Take(limit);
            var data = await query.ProjectTo<RoomCategoryDto>(_mapper.ConfigurationProvider).ToListAsync();

            return Ok(new { limit, page, total, totalPages, data });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetRoomCategoryById(Guid id)
        {
            var roomCategory = await _context.RoomCategories.ProjectTo<RoomCategoryDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(rc => rc.Id == id);

            if (roomCategory is null)
            {
                return NotFound();
            }

            return Ok(roomCategory);
        }

        [HttpPost]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> CreateRoomCategory([FromBody] AddRoomCategoryDto addRoomCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roomCategory = _mapper.Map<RoomCategory>(addRoomCategoryDto);
            _context.RoomCategories.Add(roomCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoomCategoryById), new {id = roomCategory.Id}, _mapper.Map<RoomCategoryDto>(roomCategory));
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRoomCategory(Guid id, [FromBody] UpdateRoomCategoryDto updateRoomCategoryDto)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            var roomCategory = await _context.RoomCategories.FindAsync(id);

            if (roomCategory is null)
            {
                return NotFound();
            }

            _mapper.Map(updateRoomCategoryDto, roomCategory);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<RoomCategoryDto>(roomCategory));
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveRoomCategory(Guid id)
        {
            var roomCategory = await _context.RoomCategories.FindAsync(id);

            if (roomCategory is null)
            {
                return NotFound();
            }

            _context.RoomCategories.Remove(roomCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id:guid}/rooms")]
        public async Task<IActionResult> GetRoomCategoryByIdWithRooms(Guid id)
        {
            var isCategoryExist = await _context.RoomCategories.AnyAsync(rc => rc.Id == id);

            if (!isCategoryExist) 
            {
                return NotFound( new {message = "Category not found."} );
            }

            var rooms = await _context.Rooms.Where(r => r.RoomCategoryId == id).ProjectTo<RoomWithoutCategoryDto>(_mapper.ConfigurationProvider).ToListAsync();
            return Ok(rooms);

        }



    }
}
