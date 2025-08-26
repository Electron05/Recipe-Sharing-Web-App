using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using RecipeBay.Data;
using RecipeBay.DTOs;
using RecipeBay.Mappings;

namespace RecipeBay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly RecipeBayContext _context;

        public RecipesController(RecipeBayContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeDtoPage>>> GetRecipes()
        {
            var recipes = await _context.Recipes
                .Select(r => r.ToDtoPage())
                .ToListAsync();

            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDtoPage>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes
                .Where(r => r.Id == id)
                .Select(r => r.ToDtoPage())
                .FirstOrDefaultAsync();

            if (recipe == null) return NotFound();

            return Ok(recipe);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<RecipeDtoCreate>> CreateRecipe(RecipeDtoCreate dto)
        {
            var author = await _context.Users.FindAsync(dto.AuthorId);
            if (author == null)
            {
                return BadRequest("Author not found.");
            }

            var recipe = dto.ToEntity();
            
            recipe.CreatedAt = DateTime.UtcNow;
            recipe.Author = author;
            recipe.Likes = 0;

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetRecipe),
                new { id = recipe.Id },
                recipe.ToDtoPage()
            );
        }
    }



}
