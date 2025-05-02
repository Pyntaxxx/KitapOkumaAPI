﻿namespace KitapOkumaAPI.Models
{
	public class Book
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public int AuthorId { get; set; }
		public BookAuthor Author { get; set; }

		public int GenreId { get; set; }
		public BookGenre Genre { get; set; }

		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		public string UserId { get; set; }
		public ApplicationUser User { get; set; }

		public ICollection<Note> Notes { get; set; }
	}
}
