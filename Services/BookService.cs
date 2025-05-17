using KitapOkumaAPI.Data;
using KitapOkumaAPI.Dtos;
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
                .Include(b => b.ApplicationUser)
                .ToListAsync();
        }

        public async Task<Book> AddBookAsync(BookDto bookDto)
        {
            // Yeni bir kitap nesnesi oluşturulur (DTO'dan gelen verilerle)
            var newBook = new Book
            {
                Title = bookDto.Title,
                AuthorId = bookDto.AuthorId,
                GenreId = bookDto.GenreId,
                UserId = bookDto.UserId
            };

            // Kitap veritabanına eklenir
            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            return newBook; // Eklenen yeni kitap döndürülür
        }

        public async Task<bool> UpdateBookAsync(BookDto bookDto)
        {
            // Kitap ID'sine göre mevcut kitap alınır
            var existingBook = await _context.Books.FindAsync(bookDto.Id);

            if (existingBook == null)
            {
                return false;
            }

            // Mevcut kitap güncellenir
            existingBook.Title = bookDto.Title;
            existingBook.AuthorId = bookDto.AuthorId;
            existingBook.GenreId = bookDto.GenreId;
            existingBook.UserId = bookDto.UserId;

            // Değişiklikler kaydedilir
            await _context.SaveChangesAsync();

            return true;
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