using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBay.Data;
using RecipeBay.DTOs;
using RecipeBay.Mappings;

namespace RecipeBay.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class IngredientsController : ControllerBase
	{
		private readonly RecipeBayContext _context;
		public IngredientsController(RecipeBayContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<IngredientSuggestionDto>>> GetIngredients()
		{
			var ingredients = await _context.Ingredients.Select(i => i.ToSuggestionDto()).ToListAsync();
			return Ok(ingredients);
		}

		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<IngredientSuggestionDto>>> SearchIngredients(
			[FromQuery] string name,
			[FromQuery] int limit = 10)
		{
			if (string.IsNullOrWhiteSpace(name))
				return Ok(new List<IngredientSuggestionDto>());

			name = name.ToLower();

			var ingredients = await _context.Ingredients
				.Where(i => i.Name.ToLower().Contains(name))
				.Select(i => i.ToSuggestionDto())
				.ToListAsync();

			var aliases = await _context.IngredientAliases
				.Where(a => a.Name.ToLower().Contains(name))
				.Select(a => a.ToSuggestionDto())
				.ToListAsync();

			var combined = ingredients.Concat(aliases)
				.Take(limit)
				.ToList();

			// Sort by relevance:
			var sorted = combined
				.OrderByDescending(i => i.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase)) // starts-with first
				.ThenBy(i => i.Name) // alphabetical asc
				.Take(limit)
				.ToList();

			return Ok(sorted);
		}

		[HttpGet("exists")]
		public async Task<ActionResult<int[]>> IngredientBasicIdOrAliasIds(
			[FromQuery] string name)
		{

			int[] result = { -1, -1, -1 };
			// result[2]: 1 = singular -1 = plural

			// If result [1] != -1: 
			// 	result[1] = aliasId 
			// 	result[0] = aliasParentIngredientId
			// else:
			// 	result[0] = ingredientId

			if (string.IsNullOrWhiteSpace(name))
				return Ok(result);

			var nameLower = name.ToLower();

			// Check if name matches any plural in aliases
			var aliasPlural = await _context.IngredientAliases
				.Where(a => a.Plural.ToLower() == nameLower)
				.FirstOrDefaultAsync();

			if (aliasPlural != null)
			{
				result[1] = aliasPlural.Id;
				result[0] = aliasPlural.IngredientId;
				result[2] = -1; // plural
				return Ok(result);
			}

			// Check if name matches any singular in aliases
			var aliasSingular = await _context.IngredientAliases
				.Where(a => a.Name.ToLower() == nameLower)
				.FirstOrDefaultAsync();

			if (aliasSingular != null)
			{
				result[1] = aliasSingular.Id;
				result[0] = aliasSingular.IngredientId;
				result[2] = 1; // singular
				return Ok(result);
			}

			// Check if name matches any plural in ingredients
			var ingredientPlural = await _context.Ingredients
				.Where(i => i.Plural.ToLower() == nameLower)
				.FirstOrDefaultAsync();

			if (ingredientPlural != null)
			{
				result[0] = ingredientPlural.Id;
				result[2] = -1; // plural
				return Ok(result);
			}

			// Check if name matches any singular in ingredients
			var ingredientSingular = await _context.Ingredients
				.Where(i => i.Name.ToLower() == nameLower)
				.FirstOrDefaultAsync();

			if (ingredientSingular != null)
			{
				result[0] = ingredientSingular.Id;
				result[2] = 1; // singular
				return Ok(result);
			}

			return Ok(result);
		}
	}
}