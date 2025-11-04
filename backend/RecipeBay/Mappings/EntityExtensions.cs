using RecipeBay.Models;
using RecipeBay.DTOs;


namespace RecipeBay.Mappings
{
	public static class EntityExtensions
	{
		// Recipe -> RecipeDtoDisplay
		public static RecipeDtoDisplay ToDtoDisplay(this Recipe r)
		{
			return new RecipeDtoDisplay
			{
				Id = r.Id,
				isDeleted = r.isDeleted,
				Title = r.Title,
				Description = r.Description,


				Ingredients = r.IngredientEntries.Select(ie =>
				{
					return ie.IngredientAlias?.Name
						?? ie.Ingredient?.Name
						?? ie.CustomIngredientName
						?? string.Empty;
				}).ToList(),

				// add debug if alias is null, ingredient is null, customname or is null


				IngredientsAmounts = r.IngredientEntries.Select(ie => ie.Quantity).ToList(),
				Steps = r.Steps,
				TimeToPrepareMinutes = r.TimeToPrepareMinutes,
				TimeToPrepareHours = r.TimeToPrepareHours,
				TimeToPrepareLongerThan1Day = r.TimeToPrepareLongerThan1Day,
				CreatedAt = r.CreatedAt,
				AuthorId = r.AuthorId,
				Likes = r.Likes
			};
		}

		// Recipe -> RecipeDtoFeed
		public static RecipeDtoFeed ToDtoFeed(this Recipe r)
		{
			return new RecipeDtoFeed
			{
				Id = r.Id,
				Title = r.Title,
				Description = r.Description,
				TimeToPrepareMinutes = r.TimeToPrepareMinutes,
				TimeToPrepareHours = r.TimeToPrepareHours,
				TimeToPrepareLongerThan1Day = r.TimeToPrepareLongerThan1Day,
				Difficulty = r.DifficultyFrom1To3 switch
				{
					1 => "Easy",
					2 => "Medium",
					3 => "Hard",
					_ => "Unknown"
				},
				Likes = r.Likes
			};
		}


		// User -> UserProfileDtoDisplay
		public static UserProfileDtoDisplay ToDtoDisplay(this User u)
		{
			return new UserProfileDtoDisplay
			{
				Id = u.Id,
				isDeleted = u.isDeleted,
				Username = u.Username,

				Bio = u.Bio,

				ProfilePictureUrl = u.ProfilePictureUrl,
				// Strip email and pass hash
				CreatedAt = u.CreatedAt,
				Recipes = u.Recipes?.Select(r => r.ToDtoFeed()).ToList() ?? new List<RecipeDtoFeed>(),
				FollowersCount = u.Followers.Count,
				FollowingCount = u.Following.Count
			};
		}

		// Comment -> CommentDtoDisplay
		public static CommentDtoDisplay ToDtoDisplay(this Comment c)
		{
			return new CommentDtoDisplay
			{
				Id = c.Id,
				isDeleted = c.isDeleted,
				Body = c.Body,
				RecipeId = c.RecipeId,
				AuthorId = c.AuthorId,
				CreatedAt = c.CreatedAt,
				Likes = c.Likes
			};
		}

		// Ingredient -> IngredientSuggestionDto
		public static IngredientSuggestionDto ToSuggestionDto(this Ingredient i)
		{
			return new IngredientSuggestionDto
			{
				Id = i.Id,
				Name = i.Name,
				Plural = i.Plural,
				isAlias = false,
				BaseIngredientId = i.ParentIngredientId
			};
		}

		// IngredientAlias -> IngredientSuggestionDto
		public static IngredientSuggestionDto ToSuggestionDto(this IngredientAlias ia)
		{
			return new IngredientSuggestionDto
			{
				Id = ia.Id,
				Name = ia.Name,
				Plural = ia.Plural,
				isAlias = true,
				BaseIngredientId = ia.IngredientId
			};
		}

	}
}
