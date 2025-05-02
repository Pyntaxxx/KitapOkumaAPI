using KitapOkumaAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace KitapOkumaAPI.Services
{
	public class UserService : IUserService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		// Kullanıcı kaydetme işlemi
		public async Task<ApplicationUser> RegisterUserAsync(ApplicationUser user, string password)
		{
			var result = await _userManager.CreateAsync(user, password);
			if (result.Succeeded)
			{
				return user;
			}
			else
			{
				return null;
			}
		}

		// Kullanıcı giriş yapma işlemi
		public async Task<ApplicationUser> LoginUserAsync(string username, string password)
		{
			var user = await _userManager.FindByNameAsync(username);
			if (user == null) return null;

			var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
			if (result.Succeeded)
			{
				return user;
			}

			return null;
		}

		// Kullanıcıyı ID'ye göre bulma işlemi
		public async Task<ApplicationUser> GetUserByIdAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			return user;
		}

		// Tüm kullanıcıları listeleme işlemi
		public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
		{
			return _userManager.Users;
		}

		// Kullanıcı güncelleme işlemi
		public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
		{
			var result = await _userManager.UpdateAsync(user);
			return result;
		}

		// Kullanıcı silme işlemi
		public async Task<IdentityResult> DeleteUserAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null) return IdentityResult.Failed(new IdentityError { Description = "Kullanıcı bulunamadı." });

			var result = await _userManager.DeleteAsync(user);
			return result;
		}
	}
}
