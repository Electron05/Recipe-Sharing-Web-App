using Microsoft.EntityFrameworkCore;
using RecipeBay.Data;
using RecipeBay.Models;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RecipeBay.Services
{
    public class AuthService : IAuthService
    {
        private readonly RecipeBayContext _context;
        private readonly IConfiguration _config;

        public AuthService(RecipeBayContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<string?> LoginAsync(string usernameOrEmail, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
            if (user == null) return null;

            bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!valid) return null;

            return GenerateJwtToken(user);
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");
        
            #pragma warning disable CS8604
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")
            ?? throw new InvalidOperationException("JWT_KEY environment variable is not set. Go to .env and set at least 32-char long key."));


            var claims = new[]{
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            );
            #pragma warning restore CS8604
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
