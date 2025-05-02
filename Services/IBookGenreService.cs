using KitapOkumaAPI.Models;

namespace KitapOkumaAPI.Services
{
	public interface IBookGenreService
	{
		Task<List<BookGenre>> GetGenresAsync();
		Task<BookGenre?> GetGenreByIdAsync(int id);
		Task<BookGenre> AddGenreAsync(BookGenre genre);
		Task<bool> UpdateGenreAsync(BookGenre genre);
		Task<bool> DeleteGenreAsync(int id);
	}
}
