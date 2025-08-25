using Microsoft.EntityFrameworkCore;
using RecipeBay.Data;

namespace RecipeBay.Services
{
    public class AuthService : IAuthService
    {
        private readonly RecipeBayContext _context;

        public AuthService(RecipeBayContext context)
        {
            _context = context;
        }

        public async Task<string?> LoginAsync(string usernameOrEmail, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
            if (user == null) return null;

            bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!valid) return null;

            // Generate JWT here (later)
            return "dummy-token"; // placeholder for now
        }
    }
}
