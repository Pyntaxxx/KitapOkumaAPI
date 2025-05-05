using KitapOkumaAPI.Data;
using KitapOkumaAPI.Dtos;
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

		public async Task AddUserBookAsync(int userId, int bookId, bool isRead)
		{
			var userBook = new UserBook
			{
				UserId = userId,
				BookId = bookId,
				IsRead = isRead,
				AddedDate = DateTime.UtcNow
			};

			_context.userBooks.Add(userBook);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<UserBookDto>> GetUserBooksAsync(int userId)
		{
			return await _context.userBooks
				.Include(ub => ub.Book)
				.Where(ub => ub.UserId == userId)
				.Select(ub => new UserBookDto
				{
					BookId = ub.BookId,
					Title = ub.Book.Title,
					IsRead = ub.IsRead,
					CreatedAt = ub.AddedDate
				})
				.ToListAsync();
		}


		
		public async Task<IEnumerable<UserBookDto>> GetReadBooksAsync(int userId)
		{
			return await _context.userBooks
				.Include(ub => ub.Book)
				.Where(ub => ub.UserId == userId && ub.IsRead)
				.Select(ub => new UserBookDto
				{
					BookId = ub.BookId,
					Title = ub.Book.Title,
					IsRead = ub.IsRead,
					CreatedAt = ub.AddedDate
				})
				.ToListAsync();
		}

		public async Task<IEnumerable<UserBookDto>> GetUnreadBooksAsync(int userId)
		{
			return await _context.userBooks
				.Include(ub => ub.Book)
				.Where(ub => ub.UserId == userId && !ub.IsRead)
				.Select(ub => new UserBookDto
				{
					BookId = ub.BookId,
					Title = ub.Book.Title,
					IsRead = ub.IsRead,
					CreatedAt = ub.AddedDate
				})
				.ToListAsync();
		}







	}
}
