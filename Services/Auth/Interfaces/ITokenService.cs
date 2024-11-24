namespace AchievementsPlatform.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string username, TimeSpan expiration);
        string GetTokenId(string token);
        TimeSpan GetTokenExpiration(string token);
    }
}