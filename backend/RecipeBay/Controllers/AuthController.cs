using Microsoft.AspNetCore.Mvc;
using RecipeBay.Services;
using RecipeBay.DTOs;
using RecipeBay.Mappings;
using RecipeBay.Data;

namespace RecipeBay.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly RecipeBayContext _context;

		public AuthController(IAuthService authService, RecipeBayContext context)
		{
			_authService = authService;
			_context = context;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginDto dto)
		{
			var token = await _authService.LoginAsync(dto.UsernameOrEmail, dto.Password);
			if (token == null) return Unauthorized();

			return Ok(new { token });
		}

		[HttpPost("register")]
		public async Task<ActionResult<UserProfileDtoPage>> Register(RegisterDto dto)
		{

			if (!UsernameValid(dto.Username))
				return BadRequest("Invalid username. Must be 3-20 characters, alphanumeric, underscores, or hyphens.");

			if (!EmailValid(dto.Email))
				return BadRequest("Invalid email format.");

			var user = dto.ToEntity();

			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
			user.CreatedAt = DateTime.UtcNow;

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return CreatedAtAction(
				actionName: "GetUser",
				controllerName: "Users",
				routeValues: new { id = user.Id },
				value: user.ToDtoPage()
			);
		}

		// Utility method to validate username
		private bool UsernameValid(string username)
		{
			// Check if standard characters are used
			return !string.IsNullOrWhiteSpace(username) &&
					username.Length >= 3 &&
					username.Length <= 20 &&
					username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-');
		}

		private bool EmailValid(string email)
		{
			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}

	}


	
}