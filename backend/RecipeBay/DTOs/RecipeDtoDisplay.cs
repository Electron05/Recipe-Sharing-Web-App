namespace RecipeBay.DTOs
{

	// Strip only author navigation reference
	public class RecipeDtoDisplay
	{
		public int Id { get; set; }
		public bool isDeleted { get; set; }
		public required string Title { get; set; }
		public required string Description { get; set; }
		public required List<string> Ingredients { get; set; }
		public required List<string> IngredientsAmounts { get; set; }
		public required List<string> Steps { get; set; }
		public required byte TimeToPrepareMinutes { get; set; }
		public required byte TimeToPrepareHours { get; set; }
		public required bool TimeToPrepareLongerThan1Day { get; set; }
		public DateTime CreatedAt { get; set; }
		public int? AuthorId { get; set; }
		public int Likes { get; set; }
	}
	
}