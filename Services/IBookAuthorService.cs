using KitapOkumaAPI.Models;

namespace KitapOkumaAPI.Services
{
	public interface IBookAuthorService
	{
		Task<List<BookAuthor>> GetAuthorsAsync();
		Task<BookAuthor?> GetAuthorByIdAsync(int id);
		Task<BookAuthor> AddAuthorAsync(BookAuthor author);
		Task<bool> UpdateAuthorAsync(BookAuthor author);
		Task<bool> DeleteAuthorAsync(int id);
	}
}
