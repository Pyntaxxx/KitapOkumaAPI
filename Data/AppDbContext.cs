using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KitapOkumaAPI.Models;
using System.Collections.Generic;
namespace KitapOkumaAPI.Data
{
	public class AppDbContext : DbContext
	{


		public DbSet<ApplicationUser> applicationUsers { get; set; }
		public DbSet<Book> Books { get; set; }
		public DbSet<BookAuthor> BookAuthors { get; set; }
		public DbSet<BookGenre> BookGenres { get; set; }
		public DbSet<Note> Notes { get; set; }
		public DbSet<UserBook> userBooks { get; set; }

		public AppDbContext(DbContextOptions options) : base(options)
		{

		}

	}
}
