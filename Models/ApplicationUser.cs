using Microsoft.AspNetCore.Identity;

namespace KitapOkumaAPI.Models
{
	public class ApplicationUser : IdentityUser
	{
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public ICollection<Book> Books { get; set; }
	}
}
