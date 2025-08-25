using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBay.Models;
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
        public async Task<ActionResult<IEnumerable<UserProfileDtoPage>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => u.ToDtoPage())
                .ToListAsync();

            return Ok(users);
        }

        // GET single user by id
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfileDtoPage>> GetUser(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => u.ToDtoPage())
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();
            return Ok(user);
        }

        // POST create user
        [HttpPost]
        public async Task<ActionResult<UserProfileDtoPage>> CreateUser(UserProfileDtoCreate dto)
        {
            var user = dto.ToEntity();

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user.ToDtoPage());
        }
    }
}
