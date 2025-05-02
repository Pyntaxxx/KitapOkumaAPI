using KitapOkumaAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace KitapOkumaAPI.Services
{
	public interface IUserService
	{
		Task<ApplicationUser> RegisterUserAsync(ApplicationUser user, string password);
		Task<ApplicationUser> LoginUserAsync(string username, string password);
		Task<ApplicationUser> GetUserByIdAsync(string userId);
		Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
		Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
		Task<IdentityResult> DeleteUserAsync(string userId);
	}
}
