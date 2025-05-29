using KitapOkumaAPI.Data;
using KitapOkumaAPI.Dtos;
using KitapOkumaAPI.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace KitapOkumaAPI.Services
{
	public class UserService : IUserService
	{
		private readonly AppDbContext _context;
		private readonly IConfiguration _configuration;

		public UserService(AppDbContext context, IConfiguration configuration)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		public async Task<ApplicationUser> RegisterUserAsync(RegisterDto registerDto)
		{
			if (string.IsNullOrWhiteSpace(registerDto.Email))
			{
				throw new Exception("Email alanı zorunludur.");
			}

			if (await _context.ApplicationUsers.AnyAsync(u => u.Email == registerDto.Email))
			{
				throw new Exception("Email already exists.");
			}

			var user = new ApplicationUser
			{
				UserName = registerDto.Email,
				Email = registerDto.Email,
				Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
				NamaLastName = registerDto.Email.Split('@')[0], // Varsayılan isim
				Role = registerDto.Role ?? "User",
				CreatedAt = DateTime.UtcNow
			};

			System.Diagnostics.Debug.WriteLine($"Registering user: Email={user.Email}, Role={user.Role}, Name={user.NamaLastName}");

			_context.ApplicationUsers.Add(user);
			await _context.SaveChangesAsync();

			System.Diagnostics.Debug.WriteLine($"User registered: ID={user.Id}");
			return user;
		}

		public async Task<(ApplicationUser, string)> LoginUserAsync(LoginDto loginDto)
		{
			if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
			{
				System.Diagnostics.Debug.WriteLine("Login attempt with invalid credentials.");
				return (null, null);
			}

			var user = await _context.ApplicationUsers
				.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

			if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
			{
				System.Diagnostics.Debug.WriteLine($"Login failed for email: {loginDto.Email}");
				return (null, null);
			}

			var token = GenerateJwtToken(user);
			System.Diagnostics.Debug.WriteLine($"Login successful for user: {user.Email}, Token: {token.Substring(0, Math.Min(token.Length, 20))}...");
			return (user, token);
		}

		[Authorize]
		public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
		{
			var users = await _context.ApplicationUsers.ToListAsync();
			System.Diagnostics.Debug.WriteLine($"GetAllUsersAsync: {users.Count} users found.");
			foreach (var user in users)
			{
				System.Diagnostics.Debug.WriteLine($"User: ID={user.Id}, Email={user.Email}, Role={user.Role}, Name={user.NamaLastName}");
			}
			return users;
		}

		[Authorize]
		public async Task<IEnumerable<ApplicationUser>> GetAuthorsAsync()
		{
			try
			{
				var authors = await _context.ApplicationUsers
					.Where(u => u.Role != null && u.Role.ToLower() == "author")
					.ToListAsync();
				System.Diagnostics.Debug.WriteLine($"GetAuthorsAsync: {authors.Count} authors found.");
				return authors;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"GetAuthorsAsync Error: {ex.Message}\n{ex.StackTrace}");
				throw new Exception("Yazarlar alınırken bir hata oluştu.", ex);
			}
		}

		public async Task<bool> UpdateUserAsync(int userId, UpdateUserDto dto)
		{
			var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
			if (user == null) return false;

			user.UserName = dto.UserName;
			user.NamaLastName = dto.NamaLastName;
			user.Email = dto.Email;
			if (!string.IsNullOrEmpty(dto.Password))
			{
				user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
			}

			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<bool> DeleteUserAsync(int userId)
		{
			var user = await _context.ApplicationUsers.FindAsync(userId);
			if (user == null)
			{
				System.Diagnostics.Debug.WriteLine($"User not found: ID={userId}");
				return false;
			}

			_context.ApplicationUsers.Remove(user);
			await _context.SaveChangesAsync();
			System.Diagnostics.Debug.WriteLine($"User deleted: ID={userId}");
			return true;
		}

		public string GenerateJwtToken(ApplicationUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
				new Claim(ClaimTypes.Role, user.Role ?? "User")
			};

			var keyString = _configuration["Jwt:Key"];
			if (string.IsNullOrEmpty(keyString))
			{
				System.Diagnostics.Debug.WriteLine("JWT Key is missing in configuration.");
				throw new Exception("JWT Key is not configured.");
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddDays(1),
				signingCredentials: creds);

			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
			System.Diagnostics.Debug.WriteLine($"Generated Token: {tokenString.Substring(0, Math.Min(tokenString.Length, 20))}...");
			return tokenString;
		}

		[Authorize]
		public async Task<int> GetReadBooksCountAsync(int userId)
		{
			try
			{
				var readBooksCount = await _context.UserBooks
					.Where(ub => ub.UserId == userId)
					.CountAsync();
				System.Diagnostics.Debug.WriteLine($"GetReadBooksCountAsync: User {userId} has read {readBooksCount} books.");
				return readBooksCount;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"GetReadBooksCountAsync Error: {ex.Message}\n{ex.StackTrace}");
				throw new Exception("Okunan kitap sayısı alınırken bir hata oluştu.", ex);
			}
		}

		[Authorize]
		public async Task<ApplicationUser> GetUserProfileAsync(int userId)
		{
			try
			{
				var user = await _context.ApplicationUsers
					.FirstOrDefaultAsync(u => u.Id == userId);
				if (user == null)
				{
					System.Diagnostics.Debug.WriteLine($"GetUserProfileAsync: User not found for ID={userId}");
					throw new Exception("Kullanıcı bulunamadı.");
				}

				System.Diagnostics.Debug.WriteLine($"GetUserProfileAsync: User found - ID={user.Id}, Email={user.Email}, Role={user.Role}");
				return user;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"GetUserProfileAsync Error: {ex.Message}\n{ex.StackTrace}");
				throw new Exception("Profil alınırken bir hata oluştu.", ex);
			}
		}
	}
}