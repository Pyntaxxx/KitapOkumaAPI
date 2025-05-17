using KitapOkumaAPI.Dtos;
using KitapOkumaAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KitapOkumaAPI.Services
{
    public interface IBookService
    {
        Task<List<Book>> GetBooksAsync();
        Task<Book> AddBookAsync(BookDto bookDto);
        Task<bool> UpdateBookAsync(BookDto bookDto);
        Task<bool> DeleteBookAsync(int id);
    }
}