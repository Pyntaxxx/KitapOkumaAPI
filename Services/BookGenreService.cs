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
		public async Task<BookGenre> AddGenreAsync(BookGenre genre)
		{
			var newGenre = new BookGenre
			{
				Name = genre.Name
			};
			_context.BookGenres.Add(newGenre);
			await _context.SaveChangesAsync();
			return newGenre;
		}

		public async Task<bool> UpdateGenreAsync(BookGenre genre)
		{
			var existingGenre = await _context.BookGenres.FindAsync(genre.Id);
			if (existingGenre == null)
				return false;
			existingGenre.Name = genre.Name;

			_context.BookGenres.Update(existingGenre);
			await _context.SaveChangesAsync();
			return true;

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
