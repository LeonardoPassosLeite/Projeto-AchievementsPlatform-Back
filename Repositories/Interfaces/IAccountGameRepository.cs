using AchievementsPlatform.Dtos;

namespace AchievementsPlatform.Repositories.Interfaces
{
    public interface IAccountGameRepository
    {
        Task<AccountGame?> GetGameAsync(int appId, string steamId);
        Task<IEnumerable<AccountGame>> GetGamesBySteamIdAsync(string steamId);
        Task AddGameIfNotExists(AccountGameDto gameDto, string steamId);
        Task<AccountGame?> GetGameBySteamUserIdAndAppIdAsync(string steamUserId, int appId);
    }
}
