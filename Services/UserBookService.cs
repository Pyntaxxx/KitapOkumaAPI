using KitapOkumaAPI.Data;
using KitapOkumaAPI.Models;
using Microsoft.EntityFrameworkCore;
namespace KitapOkumaAPI.Services
{
	public class UserBookService : IUserBookService
	{
		private readonly AppDbContext _context;

		public UserBookService(AppDbContext context)
		{
			_context = context;
		}

		public async Task<bool> AddBookToUserAsync(int userId, int bookId)
		{
			var userBook = new UserBook { UserId = userId, BookId = bookId };
			_context.userBooks.Add(userBook);
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<IEnumerable<Book>> GetUserBooksAsync(int userId)
		{
			return await _context.userBooks
				.Where(ub => ub.UserId == userId)
				.Include(ub => ub.Book)
				.Select(ub => ub.Book)
				.ToListAsync();
		}
	}
}
