using System.ComponentModel.DataAnnotations;
namespace RecipeBay.DTOs
{

	// Strip: Author navigation reference, CreatedAt, Likes, Id
	public class RecipeDtoCreate
	{
		[Required]
		public required string Title { get; set; }
		[Required]
		public required string Description { get; set; }
		[Required]
		public required List<string> Ingredients { get; set; }
		[Required]
		public required List<string> IngredientsAmounts { get; set; }
		[Required]
		public required List<string> Steps { get; set; }
		[Required]
		public required byte TimeToPrepareMinutes { get; set; }
		[Required]
		public required byte TimeToPrepareHours { get; set; }
		[Required]
		public required bool TimeToPrepareLongerThan1Day { get; set; }
		[Required]
		public int AuthorId { get; set; }
	}

}