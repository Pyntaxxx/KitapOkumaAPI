using KitapOkumaAPI.Data;
using KitapOkumaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KitapOkumaAPI.Services
{
	public class BookAuthorService : IBookAuthorService
	{
		private readonly AppDbContext _context;

		public BookAuthorService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<BookAuthor>> GetAuthorsAsync()
		{
			return await _context.BookAuthors.ToListAsync();
		}

		public async Task<BookAuthor?> GetAuthorByIdAsync(int id)
		{
			return await _context.BookAuthors.FindAsync(id);
		}

		public async Task<BookAuthor> AddAuthorAsync(BookAuthor author)
		{
			_context.BookAuthors.Add(author);
			await _context.SaveChangesAsync();
			return author;
		}

		public async Task<bool> UpdateAuthorAsync(BookAuthor author)
		{
			_context.BookAuthors.Update(author);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> DeleteAuthorAsync(int id)
		{
			var author = await _context.BookAuthors.FindAsync(id);
			if (author == null) return false;

			_context.BookAuthors.Remove(author);
			return await _context.SaveChangesAsync() > 0;
		}
	}
}
