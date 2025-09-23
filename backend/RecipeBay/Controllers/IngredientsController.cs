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
			[FromQuery] string query,
			[FromQuery] int limit = 10)
		{
			if (string.IsNullOrWhiteSpace(query))
				return Ok(new List<IngredientSuggestionDto>());

			query = query.ToLower();

			var ingredients = await _context.Ingredients
				.Where(i => i.Name.ToLower().Contains(query))
				.Select(i => i.ToSuggestionDto())
				.ToListAsync();

			var aliases = await _context.IngredientAliases
				.Where(a => a.Name.ToLower().Contains(query))
				.Select(a => a.ToSuggestionDto())
				.ToListAsync();

			var combined = ingredients.Concat(aliases)
				.Take(limit)
				.ToList();

			// Sort by relevance:
			var sorted = combined
				.OrderByDescending(i => i.Name.StartsWith(query, StringComparison.OrdinalIgnoreCase)) // starts-with first
				.ThenBy(i => i.Name) // alphabetical asc
				.Take(limit)
				.ToList();

            return Ok(sorted);
		}
	}
}