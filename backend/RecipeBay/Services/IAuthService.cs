namespace RecipeBay.Services
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(string usernameOrEmail, string password);
    }
}
