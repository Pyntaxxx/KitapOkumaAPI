using KitapOkumaAPI.Data;
using KitapOkumaAPI.Dtos;
using KitapOkumaAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
		public async Task<ApplicationUser> RegisterUserAsync(string userName, string email, string namaLastName, string password)
		{
			var user = new ApplicationUser
			{
				UserName = userName,
				Email = email,
				NamaLastName = namaLastName,
				Password = password // şifre hashlenmeden kaydediliyor
			};

			_context.applicationUsers.Add(user);
			await _context.SaveChangesAsync();
			return user;
		}

		public async Task<ApplicationUser> LoginUserAsync(string username, string password)
		{
			// Kullanıcıyı UserName ile bul ve şifreyi kontrol et
			var user = await _context.applicationUsers
				.FirstOrDefaultAsync(u => u.UserName == username);

			// Şifreyi düz olarak kontrol et
			if (user != null && user.Password == password)
			{
				return user;
			}

			return null; // Kullanıcı bulunamadı veya şifre eşleşmedi
		}


		public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
		{
			return await _context.applicationUsers.ToListAsync();
		}

		public async Task<bool> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
		{
			var user = await _context.applicationUsers.FindAsync(userId);
			if (user == null)
			{
				return false;  // Kullanıcı bulunamadı
			}

			user.UserName = updateUserDto.UserName;
			user.Email = updateUserDto.Email;
			user.NamaLastName = updateUserDto.NamaLastName;

			// Diğer güncellemeler

			
			await _context.SaveChangesAsync();

			return true;
		}


		public async Task<bool> DeleteUserAsync(int userId)
		{
			var user = await _context.applicationUsers.FindAsync(userId);
			if (user == null)
				return false;

			_context.applicationUsers.Remove(user);
			await _context.SaveChangesAsync();
			return true;
		}


	}
}
