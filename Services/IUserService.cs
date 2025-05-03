using KitapOkumaAPI.Dtos;
using KitapOkumaAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace KitapOkumaAPI.Services
{
	public interface IUserService
	{
		Task<ApplicationUser> RegisterUserAsync(string userName, string email, string namaLastName, string password);
		Task<ApplicationUser> LoginUserAsync(string username, string password);
		Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
		Task<bool> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
		Task<bool> DeleteUserAsync(int userId);
	}
}
