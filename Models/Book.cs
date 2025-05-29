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

		[Required]
		[MaxLength(50)]
		public string Genre { get; set; }

		[MaxLength(4000)]
		public string Content { get; set; }

		[ForeignKey("AuthorId")]
		[JsonIgnore]
		public ApplicationUser ApplicationUser { get; set; }

	}
}
