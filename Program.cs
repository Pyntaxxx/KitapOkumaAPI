
using KitapOkumaAPI.Data;
using KitapOkumaAPI.Dtos;
using KitapOkumaAPI.Models;
using KitapOkumaAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;


namespace KitapOkumaAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// DbContext ve Identity servisleri
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
			var app = builder.Build();


			// Swagger
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
		
			app.UseAuthorization();


			//Book Author 
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

			//Book Genre
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


			//Book
			app.MapGet("/books", async (IBookService service) =>
			{
				var books = await service.GetBooksAsync();
				return Results.Ok(books);
			});

			app.MapPost("/books", async (IBookService service, Book book) =>
			{
				var createdBook = await service.AddBookAsync(book);
				return createdBook != null
					? Results.Created($"/books/{createdBook.Id}", createdBook)
					: Results.BadRequest("Kitap oluþturulamadý.");
			});

			app.MapPut("/books/{id}", async (IBookService service, int id, Book book) =>
			{
				book.Id = id;  // ID'yi book nesnesine ekle
				var isUpdated = await service.UpdateBookAsync(book);
				return isUpdated
					? Results.Ok(book)
					: Results.NotFound("Kitap bulunamadý veya güncelleme baþarýsýz.");
			});

			app.MapDelete("/books/{id}", async (int id, IBookService service) =>
			{
				var success = await service.DeleteBookAsync(id);
				return success ? Results.Ok() : Results.NotFound();
			});


			//Book Notes
			app.MapGet("/notes", async (INoteService service) =>
			{
				var notes = await service.GetNotesAsync();
				return Results.Ok(notes);
			});

			app.MapPost("/notes", async (INoteService service, Note note) =>
			{
				var created = await service.AddNoteAsync(note);
				return Results.Created($"/notes/{created.Id}", created);
			});

			app.MapPut("/notes/{id}", async (int id, INoteService service, Note note) =>
			{
				if (id != note.Id)
					return Results.BadRequest("ID uyuþmazlýðý.");
				var success = await service.UpdateNoteAsync(note);
				return success ? Results.Ok(note) : Results.NotFound();
			});

			app.MapDelete("/notes/{id}", async (int id, INoteService service) =>
			{
				var success = await service.DeleteNoteAsync(id);
				return success ? Results.Ok() : Results.NotFound();
			});


			//User
			app.MapPost("/users/register", async (IUserService service, ApplicationUser user, string password) =>
			{
				// Kullanýcýyý oluþtur
				var createdUser = await service.RegisterUserAsync(user.UserName, user.Email, user.NamaLastName, password);

				return createdUser is not null
					? Results.Created($"/users/{createdUser.Id}", createdUser)
					: Results.BadRequest("Kullanýcý oluþturulamadý.");
			});



			app.MapPost("/users/login", async (IUserService service, string username, string password) =>
			{
				var user = await service.LoginUserAsync(username, password);
				return user is not null
				? Results.Ok(user)
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



			app.MapPost("/userbooks", async (IUserBookService service, int userId, int bookId, bool isRead) =>
			{
				await service.AddUserBookAsync(userId, bookId, isRead);
				return Results.Ok("Kitap iliþkilendirildi.");
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
