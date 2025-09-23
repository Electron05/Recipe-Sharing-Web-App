using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using RecipeBay.Data;
using RecipeBay.DTOs;
using RecipeBay.Mappings;
using RecipeBay.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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
        public async Task<ActionResult<IEnumerable<RecipeDtoDisplay>>> GetRecipes()
        {
            var recipes = await _context.Recipes
                .Select(r => r.ToDtoDisplay())
                .ToListAsync();

            return Ok(recipes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDtoDisplay>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes
                .Where(r => r.Id == id)
                .Select(r => r.ToDtoDisplay())
                .FirstOrDefaultAsync();

            if (recipe == null) return NotFound();

            return Ok(recipe);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<RecipeDtoCreate>> CreateRecipe(RecipeDtoCreate dto)
        {

            var userIdString = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (userIdString == null)
            {
                return Unauthorized("User ID claim not found.");
            }

            int authorId = int.Parse(userIdString);

            var author = await _context.Users.FindAsync(authorId);
            if (author == null)
            {
                return BadRequest("Author with ID from claim not found.");
            }

            var recipe = new Recipe
            {
                Title = dto.Title,
                Description = dto.Description,
                IngredientEntries = new List<IngredientEntry>(),
                Steps = dto.Steps,
                CreatedAt = DateTime.UtcNow,
                AuthorId = authorId,
                Author = author,
                Likes = 0
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetRecipe),
                new { id = recipe.Id },
                recipe.ToDtoDisplay()
            );
        }
    }



}
