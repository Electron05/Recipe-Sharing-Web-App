using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBay.Data;
using RecipeBay.DTOs;
using RecipeBay.Mappings;

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

        //CreateUser in Auth
    }
}
