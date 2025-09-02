using RecipeBay.Models;
using RecipeBay.DTOs;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;


namespace RecipeBay.Mappings
{
	public static class EntityExtensions
	{
		// Recipe → RecipeDtoDisplay
		public static RecipeDtoDisplay ToDtoDisplay(this Recipe r)
		{
			return new RecipeDtoDisplay
			{
				Id = r.Id,
				isDeleted = r.isDeleted,
				Title = r.Title,
				Description = r.Description,
				Ingredients = r.Ingredients,
				IngredientsAmounts = r.IgredientsAmounts,
				Steps = r.Steps,
				TimeToPrepareMinutes = r.TimeToPrepareMinutes,
				TimeToPrepareHours = r.TimeToPrepareHours,
				TimeToPrepareLongerThan1Day = r.TimeToPrepareLongerThan1Day,
				CreatedAt = r.CreatedAt,
				AuthorId = r.AuthorId,
				// Strip author navigation reference to avoid loops
				Likes = r.Likes
			};
		}

		// User → UserProfileDtoDisplay
		public static UserProfileDtoDisplay ToDtoDisplay(this User u)
		{
			return new UserProfileDtoDisplay
			{
				Id = u.Id,
				isDeleted = u.isDeleted,
				Username = u.Username,
				// Strip email and pass hash
				CreatedAt = u.CreatedAt,
				Recipes = u.Recipes?.Select(r => r.ToDtoDisplay()).ToList() ?? new List<RecipeDtoDisplay>()
			};
		}

		// Comment → CommentDtoDisplay
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


	}
}
