using IdentityService.Data;
using IdentityService.Models;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace IdentityService.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterUserAsync(UserRegisterDto userDto);
        Task<AuthResponseDto> LoginUserAsync(UserLoginDto loginDto);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AuthResponseDto> RegisterUserAsync(UserRegisterDto userDto)
        {
            // Check if email already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (existingUser != null)
            {
                return new AuthResponseDto
                {
                    Status = "Error",
                    Message = "This email is already registered with Indigo!"
                };
            }

            // Hash password
            var hashedPassword = BC.HashPassword(userDto.Password);

            // Create new user
            var newUser = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                HashedPassword = hashedPassword,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Status = "Success",
                Message = $"Welcome aboard, {newUser.Name}!",
                UserId = newUser.Id
            };
        }

        public async Task<AuthResponseDto> LoginUserAsync(UserLoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !BC.Verify(loginDto.Password, user.HashedPassword))
            {
                return new AuthResponseDto
                {
                    Status = "Error",
                    Message = "Invalid email or password."
                };
            }

            return new AuthResponseDto
            {
                Status = "Success",
                Message = $"Welcome back, {user.Name}!",
                UserId = user.Id
            };
        }
    }
}
