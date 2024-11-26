using AchievementsPlatform.Dtos;

namespace AchievementsPlatform.Factories
{
    public static class AccountGameFactory
    {
        public static AccountGame Create(AccountGameDto game, string steamId, string iconBaseUrl)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game));
            if (string.IsNullOrWhiteSpace(steamId))
                throw new ArgumentException("SteamId inválido.", nameof(steamId));

            return new AccountGame
            {
                AppId = game.AppId,
                Name = game.GameName,
                PlaytimeForever = game.PlaytimeForever,
                IconUrl = $"{iconBaseUrl}{game.AppId}",
                SteamUserId = steamId,
                UserAchievements = game.AchievementsCount,
                TotalAchievements = game.TotalAchievements
            };
        }
    }
}