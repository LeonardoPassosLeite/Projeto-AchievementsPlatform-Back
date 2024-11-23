using System.Text.Json;

public class SteamService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public SteamService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["SecretKeys:steamApiKey"];
    }

    public async Task<Player> GetSteamUserDetails(string steamId)
    {
        // Use _apiKey já configurada no construtor
        var apiKey = _apiKey;

        // Obtenha informações do perfil do jogador
        var profileUrl = $"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={apiKey}&steamids={steamId}";
        var profileResponse = await _httpClient.GetStringAsync(profileUrl);
        var profileData = JsonSerializer.Deserialize<SteamProfileResponse>(profileResponse);

        // Obtenha os jogos do jogador
        var gamesUrl = $"http://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={apiKey}&steamid={steamId}";
        var gamesResponse = await _httpClient.GetStringAsync(gamesUrl);
        var gamesData = JsonSerializer.Deserialize<SteamGamesResponse>(gamesResponse);

        // Combine os dados e retorne
        return new Player
        {
            SteamId = steamId,
            Name = profileData?.Response?.Players?.FirstOrDefault()?.PersonaName,
            TotalGames = gamesData?.Response?.GameCount ?? 0,
            TotalHoursPlayed = gamesData?.Response?.Games?.Sum(g => g.PlaytimeForever) ?? 0
        };
    }

    public async Task<List<GameWithAchievements>> GetUserAchievementsByGame(string steamId)
    {
        var ownedGamesUrl = $"http://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={_apiKey}&steamid={steamId}&include_appinfo=true&include_played_free_games=true";
        var gamesResponse = await _httpClient.GetAsync(ownedGamesUrl);

        if (!gamesResponse.IsSuccessStatusCode)
            throw new Exception("Erro ao buscar os jogos do usuário.");

        var gamesJson = await gamesResponse.Content.ReadAsStringAsync();
        var gamesData = JsonDocument.Parse(gamesJson);

        if (!gamesData.RootElement.TryGetProperty("response", out var responseElement) ||
            !responseElement.TryGetProperty("games", out var games))
        {
            throw new Exception("Nenhum jogo encontrado para o usuário.");
        }

        var gameList = games.EnumerateArray().Select(g => new
        {
            AppId = g.GetProperty("appid").GetInt32(),
            Name = g.GetProperty("name").GetString()
        }).ToList();

        var result = new List<GameWithAchievements>();
        foreach (var game in gameList)
        {
            var achievementsUrl = $"http://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key={_apiKey}&steamid={steamId}&appid={game.AppId}";
            var achievementsResponse = await _httpClient.GetAsync(achievementsUrl);

            if (!achievementsResponse.IsSuccessStatusCode)
                continue; // Ignorar jogos sem conquistas ou com erro

            var achievementsJson = await achievementsResponse.Content.ReadAsStringAsync();
            var achievementsData = JsonDocument.Parse(achievementsJson);

            if (!achievementsData.RootElement.TryGetProperty("playerstats", out var playerStats) ||
                !playerStats.TryGetProperty("achievements", out var achievements))
            {
                continue; // Ignorar jogos sem conquistas
            }

            var achievementList = achievements.EnumerateArray().Select(a => new Achievement
            {
                ApiName = a.GetProperty("apiname").GetString(),
                Name = a.GetProperty("name").GetString(),
                Description = a.GetProperty("description").GetString(),
                Achieved = a.GetProperty("achieved").GetInt32() == 1
            }).ToList();

            result.Add(new GameWithAchievements
            {
                GameName = game.Name,
                AppId = game.AppId,
                Achievements = achievementList
            });
        }

        return result;
    }
}