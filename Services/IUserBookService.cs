using KitapOkumaAPI.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KitapOkumaAPI.Services
{
    public interface IUserBookService
    {
        Task AddUserBookAsync(int userId, AddUserBookDto userBookDto);
        Task<IEnumerable<UserBookDto>> GetUserBooksAsync(int userId);
        Task<IEnumerable<UserBookDto>> GetReadBooksAsync(int userId);
        Task<IEnumerable<UserBookDto>> GetUnreadBooksAsync(int userId);
    }
}