using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KitapOkumaAPI.Models;
using System.Collections.Generic;
namespace KitapOkumaAPI.Data
{
	public class AppDbContext : IdentityDbContext<ApplicationUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<Book> Books { get; set; }
		public DbSet<BookAuthor> BookAuthors { get; set; }
		public DbSet<BookGenre> BookGenres { get; set; }
		public DbSet<Note> Notes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Book>()
				.Property(b => b.Title)
				.IsRequired()
				.HasMaxLength(100);
		}

	}
}
