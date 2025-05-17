using KitapOkumaAPI.Data;
using KitapOkumaAPI.Dtos;
using KitapOkumaAPI.Models;
using KitapOkumaAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace KitapOkumaAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DbContext ve servisler
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IBookAuthorService, BookAuthorService>();
            builder.Services.AddScoped<IBookGenreService, BookGenreService>();
            builder.Services.AddScoped<INoteService, NoteService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserBookService, UserBookService>();


            builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve);


            var app = builder.Build();

            // Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            // Book Author 
            app.MapGet("/authors", async (IBookAuthorService service) =>
            {
                var authors = await service.GetAuthorsAsync();
                return Results.Ok(authors);
            });

            app.MapPost("/authors", async (IBookAuthorService service, BookAuthor author) =>
            {
                var result = await service.AddAuthorAsync(author);
                return Results.Created($"/authors/{result.Id}", result);
            });

            app.MapPut("/authors/{id}", async (int id, IBookAuthorService service, BookAuthor updatedAuthor) =>
            {
                if (id != updatedAuthor.Id)
                    return Results.BadRequest("ID uyuþmuyor.");

                var success = await service.UpdateAuthorAsync(updatedAuthor);
                return success ? Results.Ok(updatedAuthor) : Results.NotFound();
            });

            app.MapDelete("/authors/{id}", async (int id, IBookAuthorService service) =>
            {
                var success = await service.DeleteAuthorAsync(id);
                return success ? Results.Ok() : Results.NotFound();
            });

            // Book Genre
            app.MapGet("/genres", async (IBookGenreService service) =>
            {
                var genres = await service.GetGenresAsync();
                return Results.Ok(genres);
            });

            app.MapPost("/genres", async (IBookGenreService service, BookGenre genre) =>
            {
                var created = await service.AddGenreAsync(genre);
                return Results.Created($"/genres/{created.Id}", created);
            });

            app.MapPut("/genres/{id}", async (int id, IBookGenreService service, BookGenre updated) =>
            {
                if (id != updated.Id)
                    return Results.BadRequest("ID uyumsuzluðu.");
                var success = await service.UpdateGenreAsync(updated);
                return success ? Results.Ok(updated) : Results.NotFound();
            });

            app.MapDelete("/genres/{id}", async (int id, IBookGenreService service) =>
            {
                var success = await service.DeleteGenreAsync(id);
                return success ? Results.Ok() : Results.NotFound();
            });

            // Book
            app.MapGet("/books", async (IBookService service) =>
            {
                var books = await service.GetBooksAsync();
                return Results.Ok(books);
            });

            app.MapPost("/books", async (IBookService service, BookDto bookDto) =>
            {
                var createdBook = await service.AddBookAsync(bookDto);
                return createdBook != null
                    ? Results.Created($"/books/{createdBook.Id}", createdBook)
                    : Results.BadRequest("Kitap oluþturulamadý.");
            });

            app.MapPut("/books/{id}", async (IBookService service, int id, BookDto bookDto) =>
            {
                bookDto.Id = id;
                var isUpdated = await service.UpdateBookAsync(bookDto);
                return isUpdated
                    ? Results.Ok(bookDto)
                    : Results.NotFound("Kitap bulunamadý veya güncelleme baþarýsýz.");
            });



            app.MapDelete("/books/{id}", async (int id, IBookService service) =>
            {
                var success = await service.DeleteBookAsync(id);
                return success ? Results.Ok() : Results.NotFound();
            });

            // Book Notes
            app.MapGet("/notes", async (INoteService service) =>
            {
                var notes = await service.GetNotesAsync();
                return Results.Ok(notes);
            });

            app.MapPost("/notes", async (INoteService service, NoteDto noteDto) =>
            {
                var created = await service.AddNoteAsync(noteDto);
                return Results.Created($"/notes/{created.Id}", created);
            });

            app.MapPut("/notes/{id}", async (int id, INoteService service, NoteDto noteDto) =>
            {
                noteDto.Id = id;  // URL'den gelen id'yi dto'ya ata
                var success = await service.UpdateNoteAsync(noteDto);
                return success ? Results.Ok(noteDto) : Results.NotFound();
            });



            app.MapDelete("/notes/{id}", async (int id, INoteService service) =>
            {
                var success = await service.DeleteNoteAsync(id);
                return success ? Results.Ok() : Results.NotFound();
            });

            // User
            app.MapPost("/users/register", async (IUserService service, RegisterDto registerDto) =>
            {
                try
                {
                    var createdUser = await service.RegisterUserAsync(registerDto);
                    return Results.Created($"/users/{createdUser.Id}", new { Message = "User registered successfully", UserId = createdUser.Id });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { Message = ex.Message });
                }
            });

            app.MapPost("/users/login", async (IUserService service, LoginDto loginDto) =>
            {
                var user = await service.LoginUserAsync(loginDto);
                return user is not null
                    ? Results.Ok(new { Message = "Login successful", UserId = user.Id })
                    : Results.Unauthorized();
            });

            app.MapGet("/users", async (IUserService service) =>
            {
                var users = await service.GetAllUsersAsync();
                return Results.Ok(users);
            });

            app.MapPut("/users/{id}", async (int id, IUserService service, UpdateUserDto updateUserDto) =>
            {
                var success = await service.UpdateUserAsync(id, updateUserDto);
                return success
                    ? Results.Ok(updateUserDto)
                    : Results.NotFound("Kullanýcý bulunamadý veya güncelleme baþarýsýz.");
            });

            app.MapDelete("/users/{userId}", async (IUserService service, int userId) =>
            {
                var success = await service.DeleteUserAsync(userId);
                return success
                    ? Results.Ok($"Kullanýcý {userId} silindi.")
                    : Results.NotFound($"Kullanýcý {userId} bulunamadý.");
            });

            // UserBooks
            // UserBooks
            app.MapPost("/userbooks", async (IUserBookService service, int userId, AddUserBookDto userBookDto) =>
            {
                try
                {
                    await service.AddUserBookAsync(userId, userBookDto);
                    return Results.Ok("Kitap iliþkilendirildi.");
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(new { Message = ex.Message });
                }
            });

            app.MapGet("/userbooks/{userId}", async (int userId, IUserBookService service) =>
            {
                var books = await service.GetUserBooksAsync(userId);
                return Results.Ok(books);
            });

            app.MapGet("/userbooks/{userId}/read", async (int userId, IUserBookService service) =>
            {
                var books = await service.GetReadBooksAsync(userId);
                return Results.Ok(books);
            });

            app.MapGet("/userbooks/{userId}/unread", async (int userId, IUserBookService service) =>
            {
                var books = await service.GetUnreadBooksAsync(userId);
                return Results.Ok(books);
            });

            app.Run();
        }
    }
}