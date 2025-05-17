using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KitapOkumaAPI.Models
{
	public class Book
	{
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string Title { get; set; }

		public int AuthorId { get; set; }
		public BookAuthor Author { get; set; }

		public int GenreId { get; set; }
		public BookGenre Genre { get; set; }

		[DataType(DataType.Date)]
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		public int UserId { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public ApplicationUser ApplicationUser { get; set; }


        [JsonIgnore]

        public ICollection<Note> Notes { get; set; }
	}
}
