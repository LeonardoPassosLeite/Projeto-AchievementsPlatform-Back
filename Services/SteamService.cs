using System.Text.Json;
using AchievementsPlatform.Services.Interfaces;

public class SteamService : ISteamService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public SteamService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _apiKey = configuration["SecretKeys:steamApiKey"]
                  ?? throw new InvalidOperationException("A chave da API da Steam não está configurada.");
    }

    public async Task<List<GameWithAchievements>> GetUserAchievementsByGame(string steamId)
    {
        var ownedGamesUrl = $"http://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={_apiKey}&steamid={steamId}&include_appinfo=true&include_played_free_games=true";
        var gamesResponse = await _httpClient.GetAsync(ownedGamesUrl);

        if (!gamesResponse.IsSuccessStatusCode)
        {
            Console.WriteLine("Erro ao buscar os jogos do usuário.");
            throw new Exception("Erro ao buscar os jogos do usuário.");
        }

        var gamesJson = await gamesResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Resposta completa da Steam API (Owned Games): {gamesJson}");

        var gamesData = JsonDocument.Parse(gamesJson);

        if (!gamesData.RootElement.TryGetProperty("response", out var responseElement) ||
            !responseElement.TryGetProperty("games", out var games))
        {
            Console.WriteLine("Erro: 'response' ou 'games' não encontrado na resposta.");
            throw new Exception("Nenhum jogo encontrado para o usuário.");
        }

        var gameList = games.EnumerateArray().Select(g => new GameWithAchievements
        {
            AppId = g.GetProperty("appid").GetInt32(),
            GameName = g.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "Nome não disponível",
            PlaytimeForever = g.TryGetProperty("playtime_forever", out var playtimeProp) ? playtimeProp.GetInt32() : 0,
            Achievements = new List<Achievement>()
        }).ToList();

        foreach (var game in gameList)
        {
            var achievementsUrl = $"http://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key={_apiKey}&steamid={steamId}&appid={game.AppId}";
            var achievementsResponse = await _httpClient.GetAsync(achievementsUrl);

            if (achievementsResponse.IsSuccessStatusCode)
            {
                var achievementsJson = await achievementsResponse.Content.ReadAsStringAsync();
                var achievementsData = JsonDocument.Parse(achievementsJson);

                if (achievementsData.RootElement.TryGetProperty("playerstats", out var playerStats) &&
                    playerStats.TryGetProperty("achievements", out var achievements))
                {
                    game.Achievements = achievements.EnumerateArray().Select(a => new Achievement
                    {
                        ApiName = a.TryGetProperty("apiname", out var apiNameProp) ? apiNameProp.GetString() : "Sem nome",
                        Name = a.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "Sem nome",
                        Description = a.TryGetProperty("description", out var descriptionProp) ? descriptionProp.GetString() : "Sem descrição",
                        Achieved = a.TryGetProperty("achieved", out var achievedProp) ? achievedProp.GetInt32() == 1 : false
                    }).ToList();
                }
                else
                {
                    Console.WriteLine($"Nenhuma conquista encontrada para o jogo {game.GameName} (AppId: {game.AppId}).");
                }
            }
        }

        return gameList;
    }
}