using KitapOkumaAPI.Dtos;
using KitapOkumaAPI.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KitapOkumaAPI.Services
{

    public interface IBookService
    {


		Task<List<Book>> GetBooksAsync();
		Task<IEnumerable<Book>> GetBooksByAuthorAsync(int userId);
		Task<Book> AddBookAsync(Book book, int userId);
		Task<bool> UpdateBookAsync(BookUpdateDto bookDto, int userId);
		Task<bool> DeleteBookAsync(int id);
		Task<List<string>> GetAvailableGenresAsync();
		Task<IEnumerable<Book>> SearchBooksAsync(string term, int userId);

	}
}

