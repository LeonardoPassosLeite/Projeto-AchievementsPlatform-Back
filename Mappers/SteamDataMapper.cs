using System.Text.Json;
using AchievementsPlatform.Exceptions;

namespace AchievementsPlatform.Mappers
{
    public static class SteamDataMapper
    {
        public static async Task<List<AccountGame>> MapOwnedGamesAsync(
            JsonDocument ownedGamesJson,
            string steamId,
            Func<string, int, Task<List<Achievement>>> getAchievementsAsync)
        {
            if (!ownedGamesJson.RootElement.TryGetProperty("response", out var responseElement) ||
                !responseElement.TryGetProperty("games", out var games))
            {
                throw new SteamDataMappingException("Nenhum jogo encontrado na resposta.");
            }

            var tasks = games.EnumerateArray().Select(async game =>
            {
                var appId = game.GetProperty("appid").GetInt32();
                var playtime = game.GetProperty("playtime_forever").GetInt32();
                var name = game.GetProperty("name").GetString() ?? "Nome não disponível";
                var iconUrl = $"https://cdn.steam.com/icons/{appId}";

                var achievements = await getAchievementsAsync(steamId, appId);

                return new AccountGame
                {
                    AppId = appId,
                    Name = name,
                    PlaytimeForever = playtime,
                    IconUrl = iconUrl,
                    SteamUserId = steamId,
                    UserAchievements = achievements.Count(a => a.Achieved),
                    TotalAchievements = achievements.Count
                };
            });

            return (await Task.WhenAll(tasks)).ToList();
        }

        public static List<Achievement> MapPlayerAchievements(JsonDocument achievementsJson)
        {
            if (!achievementsJson.RootElement.TryGetProperty("playerstats", out var playerStats) ||
                !playerStats.TryGetProperty("achievements", out var achievements))
            {
                return new List<Achievement>();
            }

            return achievements.EnumerateArray().Select(a =>
            {
                var apiName = a.TryGetProperty("apiname", out var apiNameElement) ? apiNameElement.GetString() : string.Empty;
                var name = a.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : "Sem nome";
                var description = a.TryGetProperty("description", out var descriptionElement) ? descriptionElement.GetString() : "Sem descrição";
                var achieved = a.TryGetProperty("achieved", out var achievedElement) && achievedElement.GetInt32() == 1;

                return new Achievement
                {
                    ApiName = apiName,
                    Name = name,
                    Description = description,
                    Achieved = achieved
                };
            }).ToList();
        }

    }
}
