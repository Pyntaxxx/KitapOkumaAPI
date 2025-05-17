using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using KitapOkumaAPI.Models;
using System.Collections.Generic;
namespace KitapOkumaAPI.Data
{
	public class AppDbContext : DbContext
	{


		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<Book> Books { get; set; }
		public DbSet<BookAuthor> BookAuthors { get; set; }
		public DbSet<BookGenre> BookGenres { get; set; }
		public DbSet<Note> Notes { get; set; }
		public DbSet<UserBook> UserBooks { get; set; }

		public AppDbContext(DbContextOptions options) : base(options)
		{

		}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserBook>()
                .HasOne(ub => ub.User)
                .WithMany(u => u.UserBooks)
                .HasForeignKey(ub => ub.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserBook>()
                .HasOne(ub => ub.Book)
                .WithMany()
                .HasForeignKey(ub => ub.BookId)
                .OnDelete(DeleteBehavior.NoAction);
        }



    }
}
