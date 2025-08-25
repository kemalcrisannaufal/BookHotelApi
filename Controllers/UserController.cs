using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookingHotel.Data;
using BookingHotel.Entities;
using BookingHotel.Models.ApplicationResponse;
using BookingHotel.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingHotel.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] int limit = 10,
            [FromQuery] int page = 1,
            [FromQuery] string role = ""
            )
        {
            if (limit <= 0) limit = 10;
            if (page <= 0) page = 1;

            IQueryable<User> query = _context.Users;



            if (Enum.TryParse<UserRole>(role, out var parsedRole))
            {
                query = query.Where(u => u.Role == parsedRole);
            }
            
            var total = await query.CountAsync();
            var totalPages = (int) Math.Ceiling(total / (double) limit);

            query = query.Skip((page - 1) * limit).Take(limit);
            var users = await query.ProjectTo<UserDto>(_mapper.ConfigurationProvider).ToListAsync();

            var response  = new PaginationResponse<UserDto>(limit, page, total, totalPages, users);
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        [Authorize (Roles = "SuperAdmin")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _context.Users.ProjectTo<UserDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(u => u.Id == id);
            return Ok(user);
        }

        [HttpPut("{id:guid}/roles")]
        [Authorize (Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateUserRole(Guid id, UpdateRoleDto updateRoleDto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            if (!Enum.TryParse<UserRole>(updateRoleDto.Role, out var role)) 
            {
                return BadRequest(new { message = "Role must be Admin or User." });
            }

            user.Role = role;

            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<UserDto>(user));
            
        }

        [HttpDelete("{id:guid}")]
        [Authorize (Roles = "SuperAdmin")]
        public async Task<IActionResult> RemoveUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user is null)
            {
                return NotFound();
            }

            user.Name = updateUserDto.Name ?? user.Name;
            user.Phone = updateUserDto.Phone ?? user.Phone;

            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<UserDto>(user));
        }
        
    }
}
