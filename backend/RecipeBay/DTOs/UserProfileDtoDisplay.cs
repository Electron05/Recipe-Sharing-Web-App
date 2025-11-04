namespace RecipeBay.DTOs
{
	public class UserProfileDtoDisplay
	{
		public int Id { get; set; }
		
		public bool isDeleted { get; set; }

		public required string Username { get; set; }

		public string? Bio { get; set; }

		public string? ProfilePictureUrl { get; set; }

		public DateTime CreatedAt { get; set; }
		
		public required List<RecipeDtoFeed> Recipes { get; set; }

		public int FollowersCount { get; set; }

		public int FollowingCount { get; set; }

		public bool IsFollowing { get; set; }
    }

}