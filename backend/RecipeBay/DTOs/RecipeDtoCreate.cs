using System.ComponentModel.DataAnnotations;
using RecipeBay.DTOs;
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
		public required List<string> Steps { get; set; }
		[Required]
		public required List<IngredientEntryDtoCreate> IngredientEntries { get; set; }
		[Required]
		public byte TimeToPrepareMinutes { get; set; }
		[Required]
		public byte TimeToPrepareHours { get; set; }
		[Required]
		public bool TimeToPrepareLongerThan1Day { get; set; }
		[Required]
		public required string Difficulty { get; set; }
	}

}