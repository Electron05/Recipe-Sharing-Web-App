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

        [HttpGet("requestForFeed")]
        public async Task<ActionResult<IEnumerable<RecipeDtoFeed>>> RequestRecipesForFeed(
            [FromQuery] int? bottomOfFeedRecipeId,
            [FromQuery] int howMany)
        {
            var query = _context.Recipes
                .Where(r => r.isDeleted == false);

            if (bottomOfFeedRecipeId.HasValue)
                query = query.Where(r => r.Id < bottomOfFeedRecipeId);

            var recipes = await query
                .OrderByDescending(r => r.Id)
                .Select(r => r.ToDtoFeed())
                .Take(howMany)
                .ToListAsync();

            return Ok(recipes);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDtoDisplay>> GetRecipe(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.IngredientEntries)
                    .ThenInclude(ie => ie.Ingredient)
                .Include(r => r.IngredientEntries)
                    .ThenInclude(ie => ie.IngredientAlias)
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
                Steps = dto.Steps,
                TimeToPrepareMinutes = dto.TimeToPrepareMinutes,
                TimeToPrepareHours = dto.TimeToPrepareHours,
                TimeToPrepareLongerThan1Day = dto.TimeToPrepareLongerThan1Day,
                DifficultyFrom1To3 = dto.Difficulty.ToLower() switch
                {
                    "easy" => (byte)1,
                    "medium" => (byte)2,
                    "hard" => (byte)3,
                    _ => (byte)1
                },
                AuthorId = authorId,
                Author = author
            };

            var ingredientIds = dto.IngredientEntries
                .Where(ie => ie.IngredientId.HasValue)
                .Select(ie => ie.IngredientId!.Value)
                .Distinct()
                .ToList();

            var ingredientsDict = await _context.Ingredients
                .Where(i => ingredientIds.Contains(i.Id))
                .ToDictionaryAsync(i => i.Id);

            recipe.IngredientEntries = dto.IngredientEntries.Select(ieDto => new IngredientEntry
            {
                Quantity = ieDto.Quantity,
                IsPlural = ieDto.IsPlural,
                IngredientId = ieDto.IngredientId,
                Ingredient = ieDto.IngredientId.HasValue ? ingredientsDict[ieDto.IngredientId.Value] : null,
                IngredientAliasId = ieDto.IngredientAliasId,
                NotInList = ieDto.NotInList,
                CustomIngredientName = ieDto.CustomIngredientName,
                Recipe = recipe
            }).ToList();



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
