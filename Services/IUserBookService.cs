using KitapOkumaAPI.Models;
using KitapOkumaAPI.Dtos;
namespace KitapOkumaAPI.Services
{
	public interface IUserBookService
	{
		Task AddUserBookAsync(int userId, int bookId, bool isRead);
		Task<IEnumerable<UserBookDto>> GetUserBooksAsync(int userId);
		Task<IEnumerable<UserBookDto>> GetReadBooksAsync(int userId);
		Task<IEnumerable<UserBookDto>> GetUnreadBooksAsync(int userId);
	}
}
