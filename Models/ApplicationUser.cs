using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace KitapOkumaAPI.Models
{
	public class ApplicationUser 

	{
        public int Id { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public string NamaLastName { get; set; }
		public string Role { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ICollection<Book>? Books { get; set; }

        [JsonIgnore]
        public ICollection<UserBook> UserBooks { get; set; }

    }
}
