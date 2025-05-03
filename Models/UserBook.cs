namespace KitapOkumaAPI.Models
{
	public class UserBook
	{
		public int Id { get; set; }

		public int UserId { get; set; }
		public ApplicationUser User { get; set; }

		public int BookId { get; set; }
		public Book Book { get; set; }

		public DateTime AddedDate { get; set; } = DateTime.Now;
	}
}
