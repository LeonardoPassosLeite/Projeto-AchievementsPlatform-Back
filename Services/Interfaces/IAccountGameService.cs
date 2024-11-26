using AchievementsPlatform.Dtos;

namespace AchievementsPlatform.Services.Interfaces
{
    public interface IAccountGameService
    {
        Task StoreUserGamesAsync(string steamId, CancellationToken cancellationToken = default);
        Task<IEnumerable<AccountGameDto>> GetStoredGamesAsync(string steamId, CancellationToken cancellationToken = default);
    }
}
