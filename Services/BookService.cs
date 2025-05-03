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



		public async Task<Book> AddBookAsync(Book book)
		{
			// Yeni bir kitap nesnesi oluşturulur (modelde gelen verilerle)
			var newBook = new Book
			{
				Title = book.Title,
				AuthorId = book.AuthorId,
				GenreId = book.GenreId,
				StartDate = book.StartDate,
				EndDate = book.EndDate,
				UserId = book.UserId
			};

			// Kitap veritabanına eklenir
			_context.Books.Add(newBook);
			await _context.SaveChangesAsync();

			return newBook; // Eklenen yeni kitap döndürülür
		}

		public async Task<bool> UpdateBookAsync(Book book)
		{
			// Kitap ID'sine göre mevcut kitap alınır
			var existingBook = await _context.Books
											  .Include(b => b.Author)
											  .Include(b => b.Genre)
											  .Include(b => b.User)
											  .FirstOrDefaultAsync(b => b.Id == book.Id);

			// Kitap bulunamadıysa false döndürülür
			if (existingBook == null)
			{
				return false;
			}

			// Yeni bir kitap nesnesi oluşturulur ve mevcut kitapla güncellenir
			var updatedBook = new Book
			{
				Id = book.Id, // Mevcut kitap ID'si korunur
				Title = book.Title,
				AuthorId = book.AuthorId,
				GenreId = book.GenreId,
				StartDate = book.StartDate,
				EndDate = book.EndDate,
				UserId = book.UserId
			};

			// Veritabanındaki mevcut kitabın yerine yeni kitap nesnesi atanır
			_context.Books.Update(updatedBook);
			await _context.SaveChangesAsync();

			return true; // Güncelleme başarılı oldu
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
