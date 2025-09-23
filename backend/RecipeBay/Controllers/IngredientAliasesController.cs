using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBay.Data;
using RecipeBay.DTOs;
using RecipeBay.Mappings;

namespace RecipeBay.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class IngredientAliasesController : ControllerBase
	{
		private readonly RecipeBayContext _context;
		public IngredientAliasesController(RecipeBayContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<IngredientSuggestionDto>>> GetIngredientAliases()
		{
			var ingredientAliases = await _context.IngredientAliases.Select(ia => ia.ToSuggestionDto()).ToListAsync();
			return Ok(ingredientAliases);
		}
	}
}