using KitapOkumaAPI.Data;
using KitapOkumaAPI.Dtos;
using KitapOkumaAPI.Models;
using KitapOkumaAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace KitapOkumaAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", policy =>
				{
					policy.AllowAnyOrigin()
						  .AllowAnyMethod()
						  .AllowAnyHeader();
				});
			});

			builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthorization(options =>
            {
				options.AddPolicy("AuthorOnly", policy => policy.RequireRole("Author", "Admin"));
				options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
			});

			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = "KitapOkumaAPI",
						ValidAudience = "KitapOkumaClient",
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-very-secure-secret-key-1234567890123456your-very-secure-secret-key-1234567890123456your-very-secure-secret-key-1234567890123456your-very-secure-secret-key-1234567890123456"))
					};
				});
			builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<IBookAuthorService, BookAuthorService>();
            builder.Services.AddScoped<IBookGenreService, BookGenreService>();
            builder.Services.AddScoped<INoteService, NoteService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserBookService, UserBookService>();
			builder.Services.AddHttpContextAccessor();
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
			app.UseCors("AllowAll");
			app.UseAuthentication();
			app.UseAuthorization();
			app.MapControllers();


			app.MapGet("/authors", async (IUserService service) =>
			{
				var authors = await service.GetAuthorsAsync();
				return Results.Ok(authors);
			}).RequireAuthorization();

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

			app.MapPost("/books", async (IBookService service, Book book, HttpContext context) =>
			{
				var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (string.IsNullOrEmpty(userId))
					return Results.Unauthorized();

				var createdBook = await service.AddBookAsync(book, int.Parse(userId));
				return createdBook != null
					? Results.Created($"/books/{createdBook.Id}", createdBook)
					: Results.BadRequest("Kitap oluþturulamadý.");
			}).RequireAuthorization("AuthorOnly");

			app.MapPut("/books/{id}", async (IBookService service, int id, BookUpdateDto bookDto, HttpContext context) =>
			{
				var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (string.IsNullOrEmpty(userId))
					return Results.Unauthorized();

				bookDto.Id = id;
				var isUpdated = await service.UpdateBookAsync(bookDto, int.Parse(userId));
				return isUpdated
					? Results.Ok(bookDto)
					: Results.NotFound("Kitap bulunamadý veya güncelleme baþarýsýz.");
			}).RequireAuthorization("AuthorOnly");



			app.MapDelete("/books/{id}", async (int id, IBookService service, AppDbContext context, HttpContext httpContext) =>
			{
				var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (string.IsNullOrEmpty(userId))
					return Results.Unauthorized();

				var book = await context.Books.FindAsync(id);
				if (book == null || book.AuthorId != int.Parse(userId))
					return Results.Forbid();

				var success = await service.DeleteBookAsync(id);
				return success ? Results.Ok() : Results.NotFound();
			}).RequireAuthorization("AuthorOnly");

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
					return Results.Ok(new
					{
						Message = "User registered successfully",
						UserId = createdUser.Id
					});
				}
				catch (Exception ex)
				{
					return Results.BadRequest(new { Message = ex.Message });
				}
			});


			app.MapPost("/users/login", async (IUserService service, LoginDto loginDto) =>
			{
				var (user, token) = await service.LoginUserAsync(loginDto);
				return user is not null
					? Results.Ok(new { Message = "Login successful", UserId = user.Id, User = user, Token = token })
					: Results.Unauthorized();
			});

			//app.MapGet("/users", async (IUserService service) =>
   //         {
   //             var users = await service.GetAllUsersAsync();
   //             return Results.Ok(users);
   //         });

			app.MapGet("/users", async (IUserService userService) =>
			{
				var users = await userService.GetAllUsersAsync();
				return Results.Ok(users);
			}).RequireAuthorization("AdminOnly");


			app.MapPut("/users/{id}", async (int id, IUserService service, UpdateUserDto updateUserDto) =>
            {
                var success = await service.UpdateUserAsync(id, updateUserDto);
                return success
                    ? Results.Ok(updateUserDto)
                    : Results.NotFound("Kullanýcý bulunamadý veya güncelleme baþarýsýz.");
            });

			app.MapDelete("/users/{id}", async (int id, IUserService userService) =>
			{
				var result = await userService.DeleteUserAsync(id);
				return result ? Results.Ok() : Results.NotFound();
			}).RequireAuthorization("AdminOnly");


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


			app.MapGet("/books/author/{userId}", async (int userId, IBookService service) =>
			{
				var books = await service.GetBooksByAuthorAsync(userId);
				return Results.Ok(books);
			}).RequireAuthorization("AuthorOnly");

			app.MapGet("/genres/available", async (IBookService service) =>
			{
				try
				{
					var genres = await service.GetAvailableGenresAsync();
					System.Diagnostics.Debug.WriteLine($"Genres retrieved: {string.Join(", ", genres ?? new List<string>())}");
					return genres != null ? Results.Ok(genres) : Results.NoContent();
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"Error in /genres/available: {ex.Message}\n{ex.StackTrace}");
					return Results.Problem(ex.Message);
				}
			});



			app.MapGet("/users/{userId}/profile", [Authorize] async (int userId, IUserService userService) =>
			{
				try
				{
					var user = await userService.GetUserProfileAsync(userId);
					return Results.Ok(user);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"Error in /users/{userId}/profile: {ex.Message}\n{ex.StackTrace}");
					return Results.Problem(ex.Message);
				}
			});

			app.MapGet("/books/search", async (string term, IBookService service, HttpContext context) =>
			{
				try
				{
					var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
					if (string.IsNullOrEmpty(userId))
					{
						System.Diagnostics.Debug.WriteLine("SearchBooks: Unauthorized - userId is null or empty");
						return Results.Unauthorized();
					}

					if (!int.TryParse(userId, out int parsedUserId))
					{
						System.Diagnostics.Debug.WriteLine($"SearchBooks: Invalid userId format: {userId}");
						return Results.BadRequest("Geçersiz kullanýcý ID formatý.");
					}

					var books = await service.SearchBooksAsync(term, parsedUserId);
					return Results.Ok(books);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"Error in /books/search: {ex.Message}\n{ex.StackTrace}");
					return Results.Problem($"Kitap aranýrken bir hata oluþtu: {ex.Message}");
				}
			}).RequireAuthorization("AuthorOnly");




			app.Run();
        }
    }
}