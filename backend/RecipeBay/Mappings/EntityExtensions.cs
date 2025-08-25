using RecipeBay.Models;
using RecipeBay.DTOs;
using BCrypt.Net;

namespace RecipeBay.Mappings
{
	public static class EntityExtensions
	{
		// Recipe → RecipeDto
		public static RecipeDtoPage ToDtoPage(this Recipe r)
		{
			return new RecipeDtoPage
			{
				Id = r.Id,
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


		// RecipeDtoCreate → Recipe
		public static Recipe ToEntity(this RecipeDtoCreate dto)
		{
			return new Recipe
			{
				Title = dto.Title,
				Description = dto.Description,
				Ingredients = dto.Ingredients,
				IgredientsAmounts = dto.IngredientsAmounts,
				Steps = dto.Steps,
				TimeToPrepareMinutes = dto.TimeToPrepareMinutes,
				TimeToPrepareHours = dto.TimeToPrepareHours,
				TimeToPrepareLongerThan1Day = dto.TimeToPrepareLongerThan1Day,
				AuthorId = dto.AuthorId,
			};
		}

		// User → UserProfileDto
		public static UserProfileDtoPage ToDtoPage(this User u)
		{
			return new UserProfileDtoPage
			{
				Id = u.Id,
				Username = u.Username,

				// Strip email and pass hash

				CreatedAt = u.CreatedAt,
				Recipes = u.Recipes?.Select(r => r.ToDtoPage()).ToList() ?? new List<RecipeDtoPage>()
			};
		}


		// UserProfileDtoCreate → User
		public static User ToEntity(this UserProfileDtoCreate dto)
		{
			return new User
			{
				Username = dto.Username,
				Email = dto.Email,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
				CreatedAt = DateTime.UtcNow
			};
		}
	}
}
