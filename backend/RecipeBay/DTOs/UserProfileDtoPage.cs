namespace RecipeBay.DTOs
{
	public class UserProfileDtoPage
	{
		public required int Id { get; set; }
		public required string Username { get; set; }

		public DateTime CreatedAt { get; set; }
		
		public List<RecipeDtoPage> Recipes { get; set; } = new();
    }

}