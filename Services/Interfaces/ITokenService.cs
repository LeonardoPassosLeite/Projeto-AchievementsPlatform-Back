namespace AchievementsPlatform.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string steamId, string username);
    }
}