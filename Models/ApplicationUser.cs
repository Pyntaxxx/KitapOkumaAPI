using Microsoft.AspNetCore.Identity;

namespace KitapOkumaAPI.Models
{
	public class ApplicationUser 

	{
        public int Id { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public string NamaLastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		//public ICollection<Book>? Books { get; set; }
	}
}
