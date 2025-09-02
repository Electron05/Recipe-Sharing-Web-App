using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBay.Data;
using RecipeBay.DTOs;
using RecipeBay.Mappings;
using RecipeBay.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RecipeBay.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class CommentController : ControllerBase
	{
		private readonly RecipeBayContext _context;

		public CommentController(RecipeBayContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<CommentDtoDisplay>>> GetComments()
		{
			var comments = await _context.Comments
				.Select(c => c.ToDtoDisplay())
				.ToListAsync();

			return Ok(comments);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<CommentDtoDisplay>> GetComment(int id)
		{
			var comment = await _context.Comments
				.Where(c => c.Id == id)
				.Select(c => c.ToDtoDisplay())
				.FirstOrDefaultAsync();

			if (comment == null) return NotFound();
			return Ok(comment);
		}

		//CreateUser in Auth
		[Authorize]
        [HttpPost]
        public async Task<ActionResult<CommentDtoDisplay>> CreateComment(CommentDtoCreate dto)
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

			var recipe = await _context.Recipes.FindAsync(dto.RecipeId);
			if (recipe == null)
			{
				return BadRequest("Recipe with ID from DTO not found.");
			}

            var comment = new Comment
			{
				Body = dto.Body,
				AuthorId = authorId,
				Author = author,
				RecipeId = dto.RecipeId,
				Recipe = recipe,
				CreatedAt = DateTime.UtcNow
			};

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return base.CreatedAtAction(
                nameof(GetComment),
                new { id = comment.Id },
                (object)comment.ToDtoDisplay()
            );
        }
	}
}