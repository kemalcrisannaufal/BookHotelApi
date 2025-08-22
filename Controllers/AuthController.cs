using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookingHotel.Data;
using BookingHotel.Entities;
using BookingHotel.Models.Auth;
using BookingHotel.Models.User;
using BookingHotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookingHotel.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly JwtService _jwtService;

        public AuthController(ApplicationDbContext context, IMapper mapper, JwtService jwtService)
        {
            _context = context;
            _mapper = mapper;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var isEmailAlreadyRegistered = await _context.Users.AnyAsync(u => u.Email == registerDto.Email);

            if (isEmailAlreadyRegistered)
            {
                return BadRequest(new { message = "Email already registered." });
            }

            var user = _mapper.Map<User>(registerDto);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await _context.Users.Where(u => u.Email == login.Email).FirstOrDefaultAsync();

            if (user is null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Email or password is incorrect." });
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(new { token });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized(new { message = "Invalid user id." });
            }

            var user = await _context.Users
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return Ok(user);
        }

        [HttpPut("update-password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto updatePasswordDto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out var userId)) {
                return Unauthorized(new { message = "Invalid user id." });
            }

            var user = await _context.Users.FindAsync(userId);

            if (user is null)
            {
                return NotFound();
            }

            if (!BCrypt.Net.BCrypt.Verify(updatePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return BadRequest(new {message = "Current password is incorrect."});
            }

            _mapper.Map(updatePasswordDto, user);
            await _context.SaveChangesAsync();

            return Ok(new {message = "Update password successfully."});
        }
    }
}
