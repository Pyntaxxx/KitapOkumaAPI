namespace KitapOkumaAPI.Dtos
{
	public class UserBookDto
	{
		public int BookId { get; set; }
		public string Title { get; set; }
		public bool IsRead { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
