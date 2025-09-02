namespace RecipeBay.DTOs
{
	public class UserProfileDtoDisplay
	{
		public int Id { get; set; }
		
		public bool isDeleted { get; set; }

		public required string Username { get; set; }

		public DateTime CreatedAt { get; set; }
		
		public required List<RecipeDtoDisplay> Recipes { get; set; }
    }

}