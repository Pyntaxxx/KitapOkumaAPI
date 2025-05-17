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

        public async Task AddUserBookAsync(int userId, AddUserBookDto userBookDto)
        {
            // Kullanıcı ve kitap var mı kontrol et
            var userExists = await _context.ApplicationUsers.AnyAsync(u => u.Id == userId);
            var bookExists = await _context.Books.AnyAsync(b => b.Id == userBookDto.BookId);

            if (!userExists || !bookExists)
            {
                throw new Exception("Kullanıcı veya kitap bulunamadı.");
            }

            // Zaten ilişki var mı kontrol et
            var existingUserBook = await _context.UserBooks
                .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BookId == userBookDto.BookId);

            if (existingUserBook != null)
            {
                throw new Exception("Bu kitap zaten kullanıcıyla ilişkilendirilmiş.");
            }

            var userBook = new UserBook
            {
                UserId = userId,
                BookId = userBookDto.BookId,
                IsRead = userBookDto.IsRead,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserBooks.Add(userBook);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserBookDto>> GetUserBooksAsync(int userId)
        {
            return await _context.UserBooks
                .Where(ub => ub.UserId == userId)
                .Include(ub => ub.Book)
                .Select(ub => new UserBookDto
                {
                    BookId = ub.BookId,
                    Title = ub.Book.Title,
                    IsRead = ub.IsRead,
                    CreatedAt = ub.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<UserBookDto>> GetReadBooksAsync(int userId)
        {
            return await _context.UserBooks
                .Where(ub => ub.UserId == userId && ub.IsRead)
                .Include(ub => ub.Book)
                .Select(ub => new UserBookDto
                {
                    BookId = ub.BookId,
                    Title = ub.Book.Title,
                    IsRead = ub.IsRead,
                    CreatedAt = ub.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<UserBookDto>> GetUnreadBooksAsync(int userId)
        {
            return await _context.UserBooks
                .Where(ub => ub.UserId == userId && !ub.IsRead)
                .Include(ub => ub.Book)
                .Select(ub => new UserBookDto
                {
                    BookId = ub.BookId,
                    Title = ub.Book.Title,
                    IsRead = ub.IsRead,
                    CreatedAt = ub.CreatedAt
                })
                .ToListAsync();
        }
    }
}