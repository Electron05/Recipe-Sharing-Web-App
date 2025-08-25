using System.ComponentModel.DataAnnotations;

namespace RecipeBay.DTOs
{
	public class LoginDto
	{
		[Required]
		public required string UsernameOrEmail { get; set; }
		[Required]
		public required string Password { get; set; }
	}
}