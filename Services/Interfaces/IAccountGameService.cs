namespace AchievementsPlatform.Services.Interfaces
{
    public interface IAccountGameService
    {
        Task StoreUserGamesAsync(string steamId);
    }
}