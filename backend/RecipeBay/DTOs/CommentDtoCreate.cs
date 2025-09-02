using System.ComponentModel.DataAnnotations;

namespace RecipeBay.DTOs
{
	public class CommentDtoCreate
	{
		[Required]
		[MaxLength(4096)]
		public required string Body { get; set; }

		[Required]
		public int RecipeId { get; set; }
	}
}