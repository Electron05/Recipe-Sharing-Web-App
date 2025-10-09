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
			[FromQuery] bool isPlural,
			[FromQuery] int limit = 10)
		{
			if (string.IsNullOrWhiteSpace(name))
				return Ok(new List<IngredientSuggestionDto>());

			name = name.ToLower();

			var ingredients = await _context.Ingredients
				.Where(i => (isPlural ? i.Plural.ToLower() : i.Name.ToLower()).Contains(name))
				.Select(i => i.ToSuggestionDto())
				.ToListAsync();

			var aliases = await _context.IngredientAliases
				.Where(a => (isPlural ? a.Plural.ToLower() : a.Name.ToLower()).Contains(name))
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
			[FromQuery] string name,
			[FromQuery] bool isPlural)
		{

			int[] result = { -1, -1 };
			// If result [1] != -1: 
			// result[1] = aliasId 
			// result[0] = parentIngredientId
			// else:
			// result[0] = ingredientId

			if (string.IsNullOrWhiteSpace(name))
				return Ok(result);

			var nameLower = name.ToLower();

			var alias = await _context.IngredientAliases
				.Where(a => (isPlural ? a.Plural.ToLower() : a.Name.ToLower()) == nameLower)
				.FirstOrDefaultAsync();

			if (alias != null)
			{
				result[1] = alias.Id;
				result[0] = alias.IngredientId;
				return Ok(result);
			}

			var ingredientId = await _context.Ingredients
				.Where(i => (isPlural ? i.Plural.ToLower() : i.Name.ToLower()) == nameLower)
				.Select(i => i.Id)
				.FirstOrDefaultAsync();

			if (ingredientId != 0)
				result[0] = ingredientId;

			return Ok(result);
		}

		[HttpGet("toSingularOrPlural")]
		public async Task<ActionResult<string>> ToSingularOrPlural(
			[FromQuery] string name,
			[FromQuery] bool isPlural)
		{
			if (string.IsNullOrWhiteSpace(name))
				return BadRequest("Changing countable form failed: empty string");

			var nameLower = name.ToLower();

			var ingredientForm = await _context.Ingredients
				.Where(i => (isPlural ? i.Plural.ToLower() : i.Name.ToLower()) == nameLower)
				.Select(i => isPlural ? i.Name : i.Plural)
				.FirstOrDefaultAsync();
			if(ingredientForm?.Length>0)
				return Ok(ingredientForm);
			var aliasForm = await _context.IngredientAliases
				.Where(a => (isPlural ? a.Plural.ToLower() : a.Name.ToLower()) == nameLower)
				.Select(a => isPlural ? a.Name : a.Plural)
				.FirstOrDefaultAsync();
			if(aliasForm?.Length >0)
				return Ok(aliasForm);
			return NotFound();
		}
		
	}
}