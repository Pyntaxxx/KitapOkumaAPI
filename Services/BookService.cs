using KitapOkumaAPI.Data;

using KitapOkumaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KitapOkumaAPI.Dtos;
using System.Security.Claims;
namespace KitapOkumaAPI.Services
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }

		public readonly List<string> _validGenres = new List<string>
		{
			"Roman",
			"Bilim Kurgu",
			"Fantastik",
			"Polisiye",
			"Biyografi",
			"Tarih",
			"Korku",
			"Macera",
			"Şiir",
			"Deneme"
		};

		[Authorize]
		[HttpGet("books")]
		public async Task<List<Book>> GetBooksAsync()
		{
			try
			{
				var books = await _context.Books
					.Include(b => b.ApplicationUser)
					.ToListAsync();
				System.Diagnostics.Debug.WriteLine($"GetBooksAsync: {books.Count} kitap bulundu.");
				return books;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"GetBooksAsync Hata: {ex.Message}\n{ex.StackTrace}");
				throw new Exception("Kitaplar alınırken bir hata oluştu.", ex);
			}
		}

		public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(int userId)
		{
			try
			{
				var books = await _context.Books
					.Where(b => b.AuthorId == userId)
					.Include(b => b.ApplicationUser)
					.ToListAsync();
				System.Diagnostics.Debug.WriteLine($"GetBooksByAuthorAsync: {books.Count} kitap bulundu, UserId={userId}.");
				return books;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"GetBooksByAuthorAsync Hata: {ex.Message}\n{ex.StackTrace}");
				throw new Exception("Yazarın kitapları alınırken bir hata oluştu.", ex);
			}
		}

		[Authorize("AuthorOnly")]
		public async Task<Book> AddBookAsync(Book book, int userId)
		{
			try
			{
				// Tür doğrulama
				if (!_validGenres.Contains(book.Genre, StringComparer.OrdinalIgnoreCase))
				{
					throw new ArgumentException($"Geçersiz kitap türü: {book.Genre}. Geçerli türler: {string.Join(", ", _validGenres)}");
				}

				// Yazar doğrulama
				var user = await _context.ApplicationUsers.FindAsync(userId);
				if (user == null || user.Role?.ToLower() != "author")
				{
					throw new ArgumentException($"Geçersiz yazar: UserId={userId} bir yazar değil.");
				}

				// Kitap nesnesi
				book.AuthorId = userId;
				book.Id = 0; // Yeni kitap için ID sıfırlanır

				_context.Books.Add(book);
				await _context.SaveChangesAsync();

				System.Diagnostics.Debug.WriteLine($"Kitap eklendi: ID={book.Id}, Başlık={book.Title}, AuthorId={book.AuthorId}");
				return book;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"AddBookAsync Hata: {ex.Message}\n{ex.StackTrace}");
				throw new Exception("Kitap eklenirken bir hata oluştu.", ex);
			}
		}

		[Authorize("AuthorOnly")]
		public async Task<bool> UpdateBookAsync(BookUpdateDto bookDto, int userId)
		{
			try
			{
				var existingBook = await _context.Books.FindAsync(bookDto.Id);
				if (existingBook == null)
				{
					System.Diagnostics.Debug.WriteLine($"Kitap bulunamadı: ID={bookDto.Id}");
					return false;
				}

				// Tür doğrulama
				if (!_validGenres.Contains(bookDto.Genre, StringComparer.OrdinalIgnoreCase))
				{
					throw new ArgumentException($"Geçersiz kitap türü: {bookDto.Genre}. Geçerli türler: {string.Join(", ", _validGenres)}");
				}

				// Yazar doğrulama
				var user = await _context.ApplicationUsers.FindAsync(userId);
				if (user == null || user.Role?.ToLower() != "author")
				{
					throw new ArgumentException($"Geçersiz yazar: UserId={userId} bir yazar değil.");
				}

				// Yetki kontrolü
				if (existingBook.AuthorId != userId)
				{
					throw new UnauthorizedAccessException("Bu kitabı güncelleme yetkiniz yok.");
				}

				// Kitabı güncelle
				existingBook.Title = bookDto.Title;
				existingBook.Genre = bookDto.Genre;
				existingBook.Content = bookDto.Content;

				await _context.SaveChangesAsync();
				System.Diagnostics.Debug.WriteLine($"Kitap güncellendi: ID={existingBook.Id}");
				return true;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"UpdateBookAsync Hata: {ex.Message}\n{ex.StackTrace}");
				throw new Exception("Kitap güncellenirken bir hata oluştu.", ex);
			}
		}


		[Authorize("AuthorOnly")]
		public async Task<bool> DeleteBookAsync(int id)
		{
			try
			{
				var book = await _context.Books.FindAsync(id);
				if (book == null)
				{
					System.Diagnostics.Debug.WriteLine($"Kitap bulunamadı: ID={id}");
					return false;
				}

				_context.Books.Remove(book);
				await _context.SaveChangesAsync();
				System.Diagnostics.Debug.WriteLine($"Kitap silindi: ID={id}");
				return true;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"DeleteBookAsync Hata: {ex.Message}\n{ex.StackTrace}");
				throw new Exception("Kitap silinirken bir hata oluştu.", ex);
			}
		}


		public async Task<IEnumerable<Book>> SearchBooksAsync(string term, int userId)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(term))
				{
					System.Diagnostics.Debug.WriteLine($"SearchBooksAsync: Term is empty, returning empty list for userId={userId}");
					return [];
				}

				var books = await _context.Books
					.Where(b => EF.Functions.Like(b.Title, $"%{term}%") && b.AuthorId == userId)
					.ToListAsync();
				System.Diagnostics.Debug.WriteLine($"SearchBooksAsync: Found {books.Count} books for term '{term}' and AuthorId={userId}");
				return books;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"SearchBooksAsync Error: {ex.Message}\n{ex.StackTrace}");
				throw new Exception($"Kitap aranırken bir hata oluştu: Term={term}, UserId={userId}, InnerException={ex.Message}", ex);
			}
		}

		public async Task<List<string>> GetAvailableGenresAsync()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine($"GetAvailableGenresAsync: {_validGenres.Count} tür döndürüldü.");
				return _validGenres; 
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"GetAvailableGenresAsync Hata: {ex.Message}\n{ex.StackTrace}");
				throw new Exception("Türler alınırken bir hata oluştu.", ex);
			}
		}


	}
}