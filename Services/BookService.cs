using KitapOkumaAPI.Data;
using KitapOkumaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KitapOkumaAPI.Services
{
	public class BookService : IBookService
	{
		private readonly AppDbContext _context;

		public BookService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<List<Book>> GetBooksAsync()
		{
			return await _context.Books
				.Include(b => b.Author)
				.Include(b => b.Genre)
				.Include(b => b.User)
				.ToListAsync();
		}

		public async Task<Book?> GetBookByIdAsync(int id)
		{
			return await _context.Books
				.Include(b => b.Author)
				.Include(b => b.Genre)
				.Include(b => b.User)
				.FirstOrDefaultAsync(b => b.Id == id);
		}

		public async Task<Book> AddBookAsync(Book book)
		{
			_context.Books.Add(book);
			await _context.SaveChangesAsync();
			return book;
		}

		public async Task<bool> UpdateBookAsync(Book book)
		{
			_context.Books.Update(book);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> DeleteBookAsync(int id)
		{
			var book = await _context.Books.FindAsync(id);
			if (book == null) return false;

			_context.Books.Remove(book);
			return await _context.SaveChangesAsync() > 0;
		}
	}
}
