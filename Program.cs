
using KitapOkumaAPI.Data;
using KitapOkumaAPI.Models;
using KitapOkumaAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


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

			builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders();
			builder.Services.AddAuthorization();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddScoped<IBookService, BookService>();
			builder.Services.AddScoped<IBookAuthorService, BookAuthorService>();
			builder.Services.AddScoped<IBookGenreService, BookGenreService>();
			builder.Services.AddScoped<INoteService, NoteService>();
			builder.Services.AddScoped<IUserService, UserService>();
			var app = builder.Build();

			// Swagger
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			app.UseAuthentication();
			app.UseAuthorization();

			app.Run();
		}
	}
}
