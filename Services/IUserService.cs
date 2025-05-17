using KitapOkumaAPI.Dtos;
using KitapOkumaAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KitapOkumaAPI.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> RegisterUserAsync(RegisterDto registerDto);
        Task<ApplicationUser> LoginUserAsync(LoginDto loginDto);
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<bool> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int userId);
    }
}