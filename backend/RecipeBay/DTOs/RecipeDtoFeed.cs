namespace RecipeBay.DTOs
{
	public class RecipeDtoFeed
	{
		public int Id { get; set; }
		public required string Title { get; set; }
		public required string Description { get; set; }
		public required byte TimeToPrepareMinutes { get; set; }
		public required byte TimeToPrepareHours { get; set; }
		public required bool TimeToPrepareLongerThan1Day { get; set; }
		public required string Difficulty { get; set; }
		public int Likes { get; set; }
	}
}