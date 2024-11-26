namespace AchievementsPlatform.Services.Interfaces
{
    public interface ISteamService
    {
        Task<List<AccountGame>> GetOwnedGamesAsync(string steamId, CancellationToken cancellationToken = default);
        Task<List<Achievement>> GetPlayerAchievementsAsync(string steamId, int appId, CancellationToken cancellationToken = default);
    }
}