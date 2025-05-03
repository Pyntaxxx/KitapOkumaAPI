using KitapOkumaAPI.Models;

namespace KitapOkumaAPI.Services
{
	public interface IUserBookService
	{
		Task<bool> AddBookToUserAsync(int userId, int bookId);
		Task<IEnumerable<Book>> GetUserBooksAsync(int userId);
	}
}
