using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SteamGamesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public SteamGamesController(AppDbContext context, IConfiguration configuration, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
        _apiKey = configuration["SecretKeys:steamApiKey"];
    }

    [HttpPost("{steamId}/save-games-with-achievements")]
    public async Task<IActionResult> SaveSteamGamesWithAchievements(string steamId)
    {
        var url = $"http://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={_apiKey}&steamid={steamId}&include_appinfo=true&include_played_free_games=true";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return BadRequest("Erro ao acessar a API da Steam para buscar jogos.");

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonDocument.Parse(json);

        if (!data.RootElement.TryGetProperty("response", out var responseElement) ||
            !responseElement.TryGetProperty("games", out var games))
        {
            return NotFound("Nenhum jogo encontrado para este usuário.");
        }

        foreach (var gameElement in games.EnumerateArray())
        {
            var appId = gameElement.GetProperty("appid").GetInt32();
            var hasAchievements = gameElement.TryGetProperty("has_community_visible_stats", out var visibleStats) &&
                                  visibleStats.GetBoolean();

            // Pular jogos que não têm conquistas
            if (!hasAchievements) continue;

            // Obter conquistas do jogo e do usuário
            var achievementsUrl = $"http://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key={_apiKey}&steamid={steamId}&appid={appId}";
            var achievementsResponse = await _httpClient.GetAsync(achievementsUrl);

            if (!achievementsResponse.IsSuccessStatusCode) continue;

            var achievementsJson = await achievementsResponse.Content.ReadAsStringAsync();
            var achievementsData = JsonDocument.Parse(achievementsJson);

            if (!achievementsData.RootElement.TryGetProperty("playerstats", out var playerStats) ||
                !playerStats.TryGetProperty("achievements", out var achievements))
            {
                continue;
            }

            var totalAchievements = achievements.GetArrayLength();
            var userAchievements = achievements.EnumerateArray().Count(a => a.GetProperty("achieved").GetInt32() == 1);

            // Salvar no banco de dados
            var game = _context.Games.FirstOrDefault(g => g.AppId == appId && g.SteamUserId == steamId);
            if (game == null)
            {
                game = new Game
                {
                    AppId = appId,
                    Name = gameElement.GetProperty("name").GetString(),
                    PlaytimeForever = gameElement.GetProperty("playtime_forever").GetInt32(),
                    IconUrl = gameElement.GetProperty("img_icon_url").GetString(),
                    SteamUserId = steamId,
                    TotalAchievements = totalAchievements,
                    UserAchievements = userAchievements
                };
                _context.Games.Add(game);
            }
            else
            {
                game.TotalAchievements = totalAchievements;
                game.UserAchievements = userAchievements;
                _context.Games.Update(game);
            }
        }

        await _context.SaveChangesAsync();

        return Ok("Jogos com conquistas salvos no banco de dados.");
    }
}