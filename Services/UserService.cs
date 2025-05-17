using KitapOkumaAPI.Data;
using KitapOkumaAPI.Dtos;
using KitapOkumaAPI.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace KitapOkumaAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        // Kullanıcı kaydetme işlemi
        public async Task<ApplicationUser> RegisterUserAsync(RegisterDto registerDto)
        {
            // Check if email already exists
            if (await _context.ApplicationUsers.AnyAsync(u => u.Email == registerDto.Email))
            {
                throw new Exception("Email already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Email, // Use email as username for simplicity
                Email = registerDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password), // Hash the password
                NamaLastName = "", // Optional: Can be updated later
                CreatedAt = DateTime.UtcNow
            };

            _context.ApplicationUsers.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Kullanıcı giriş işlemi
        public async Task<ApplicationUser> LoginUserAsync(LoginDto loginDto)
        {
            // Kullanıcıyı Email ile bul
            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            // Şifreyi kontrol et
            if (user != null && BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return user;
            }

            return null; // Kullanıcı bulunamadı veya şifre eşleşmedi
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _context.ApplicationUsers.ToListAsync();
        }

        public async Task<bool> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _context.ApplicationUsers.FindAsync(userId);
            if (user == null)
            {
                return false; // Kullanıcı bulunamadı
            }

            user.UserName = updateUserDto.UserName;
            user.Email = updateUserDto.Email;
            user.NamaLastName = updateUserDto.NamaLastName;

            // 🔐 Şifre güncelleniyorsa hashle
            if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.ApplicationUsers.FindAsync(userId);
            if (user == null)
                return false;

            _context.ApplicationUsers.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}