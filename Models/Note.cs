namespace KitapOkumaAPI.Models
{
	public class Note
	{
		public int Id { get; set; }

		public int BookId { get; set; }
		public Book Book { get; set; }

		public string Content { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
