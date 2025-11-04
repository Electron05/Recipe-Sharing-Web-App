using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using RecipeBay.Data;
using RecipeBay.DTOs;
using RecipeBay.Mappings;
using System.IdentityModel.Tokens.Jwt;

namespace RecipeBay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly RecipeBayContext _context;

        public UsersController(RecipeBayContext context)
        {
            _context = context;
        }

        // GET all users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfileDtoDisplay>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => u.ToDtoDisplay())
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("profile/{id}")]
        public async Task<ActionResult<UserProfileDtoDisplay>> GetProfile(int id)
        {
            var user = await _context.Users
                .Include(u => u.Recipes)
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var dto = user.ToDtoDisplay();

            // Check if current user is following this profile
            var userIdString = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (int.TryParse(userIdString, out int currentUserId))
            {
                dto.IsFollowing = user.Followers.Any(f => f.Id == currentUserId);
            }
            else
            {
                dto.IsFollowing = false;
            }

            return Ok(dto);
        }

        [HttpPost("{id}/follow")]
        public async Task<IActionResult> Follow(int id)
        {
            // id = user to follow; get current user from JWT sub claim
            var userIdString = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userIdString == null) return Unauthorized("User ID claim not found.");
            if (!int.TryParse(userIdString, out int currentUserId)) return Unauthorized("Invalid user id claim.");
            if (currentUserId == id) return BadRequest("Cannot follow yourself");

            var userToFollow = await _context.Users.Include(u => u.Followers).FirstOrDefaultAsync(u => u.Id == id);
            var currentUser = await _context.Users.Include(u => u.Following).FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (userToFollow == null || currentUser == null) return NotFound();

            if (userToFollow.Followers.Any(f => f.Id == currentUserId))
                return BadRequest("Already following");

            userToFollow.Followers.Add(currentUser);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("{id}/unfollow")]
        public async Task<IActionResult> Unfollow(int id)
        {
            var userIdString = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userIdString == null) return Unauthorized("User ID claim not found.");
            if (!int.TryParse(userIdString, out int currentUserId)) return Unauthorized("Invalid user id claim.");

            var user = await _context.Users.Include(u => u.Followers).FirstOrDefaultAsync(u => u.Id == id);
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);
            if (user == null || currentUser == null) return NotFound();

            var existing = user.Followers.FirstOrDefault(f => f.Id == currentUserId);
            if (existing == null) return NotFound();

            user.Followers.Remove(existing);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // GET single user by id
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfileDtoDisplay>> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => u.ToDtoDisplay())
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost("setbio")]
        public async Task<ActionResult<UserProfileDtoDisplay>> SetBio([FromBody] DTOs.SetBioDto dto)
        {
            if (dto == null) return BadRequest("Request body is required.");

            var newBio = (dto.Bio ?? string.Empty).Trim();
            if (newBio.Length > 500) return BadRequest("Bio too long (max 500 characters).");

            var userIdString = HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (userIdString == null) return Unauthorized("User ID claim not found.");
            if (!int.TryParse(userIdString, out int currentUserId)) return Unauthorized("Invalid user id claim.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);
            if (user == null) return NotFound();

            user.Bio = newBio;
            _context.Entry(user).Property(u => u.Bio).IsModified = true;
            await _context.SaveChangesAsync();

            var userWithRecipes = await _context.Users
                .Include(u => u.Recipes)
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            var profileDto = userWithRecipes!.ToDtoDisplay();
            profileDto.IsFollowing = false; // Can't follow yourself
            return Ok(profileDto);
        }
    }
}
