namespace AchievementsPlatform.Services.Auth.Interfaces
{
    public interface IAuthenticationService
    {
        string GenerateToken(string userId, string username, TimeSpan expiration);
        string GetTokenId(string token);
        TimeSpan GetTokenExpiration(string token);
    }
}