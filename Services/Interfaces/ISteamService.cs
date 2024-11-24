namespace AchievementsPlatform.Services.Interfaces
{
    public interface ISteamService
    {
        Task<List<GameWithAchievements>> GetUserAchievementsByGame(string steamId);
    }
}