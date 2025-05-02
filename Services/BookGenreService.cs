using KitapOkumaAPI.Data;
using KitapOkumaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KitapOkumaAPI.Services
{
	public class BookGenreService : IBookGenreService
	{
		private readonly AppDbContext _context;

		public BookGenreService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<BookGenre>> GetGenresAsync()
		{
			return await _context.BookGenres.ToListAsync();
		}

		public async Task<BookGenre?> GetGenreByIdAsync(int id)
		{
			return await _context.BookGenres.FindAsync(id);
		}

		public async Task<BookGenre> AddGenreAsync(BookGenre genre)
		{
			_context.BookGenres.Add(genre);
			await _context.SaveChangesAsync();
			return genre;
		}

		public async Task<bool> UpdateGenreAsync(BookGenre genre)
		{
			_context.BookGenres.Update(genre);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> DeleteGenreAsync(int id)
		{
			var genre = await _context.BookGenres.FindAsync(id);
			if (genre == null) return false;

			_context.BookGenres.Remove(genre);
			return await _context.SaveChangesAsync() > 0;
		}
	}
}
