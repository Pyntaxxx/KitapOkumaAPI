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

		public async Task<BookAuthor> AddAuthorAsync(BookAuthor author)
		{
			var newAuthor = new BookAuthor
			{
				Name = author.Name
			};
			_context.BookAuthors.Add(newAuthor);
			await _context.SaveChangesAsync();
			return newAuthor;
		}


		public async Task<bool> UpdateAuthorAsync(BookAuthor author)
		{
			var existingAuthor = await _context.BookAuthors.FindAsync(author.Id);
			if (existingAuthor == null)
				return false;
			existingAuthor.Name = author.Name;

			_context.BookAuthors.Update(existingAuthor);
			await _context.SaveChangesAsync();
			return true;
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
