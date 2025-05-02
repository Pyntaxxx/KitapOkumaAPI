using KitapOkumaAPI.Models;

namespace KitapOkumaAPI.Services
{
	public interface IBookService
	{
		Task<List<Book>> GetBooksAsync();
		Task<Book?> GetBookByIdAsync(int id);
		Task<Book> AddBookAsync(Book book);
		Task<bool> UpdateBookAsync(Book book);
		Task<bool> DeleteBookAsync(int id);
	}
}
