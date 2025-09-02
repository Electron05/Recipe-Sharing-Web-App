using System.ComponentModel.DataAnnotations;

namespace RecipeBay.DTOs
{
	public class CommentDtoDisplay
	{
		public required int Id { get; set; }

		public bool isDeleted { get; set; }

		[MaxLength(4096)]
		public required string Body { get; set; }

		public required int RecipeId { get; set; }

		public required int AuthorId { get; set; }

		public required DateTime CreatedAt { get; set; }

		public required int Likes { get; set; }
	}
}