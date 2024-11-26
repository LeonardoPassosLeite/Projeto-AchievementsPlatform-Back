using System.Text.Json;
using AchievementsPlatform.Mappers;
using AchievementsPlatform.Services.Interfaces;

public class SteamService : ISteamService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public SteamService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
        _apiKey = configuration["SecretKeys:steamApiKey"]
                  ?? throw new InvalidOperationException("A chave da API da Steam não está configurada.");
    }

    public async Task<List<AccountGame>> GetOwnedGamesAsync(string steamId, CancellationToken cancellationToken = default)
    {
        var url = $"http://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={_apiKey}&steamid={steamId}&include_appinfo=true&include_played_free_games=true";

        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new SteamApiException($"Erro ao buscar jogos do usuário. Status: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonDocument = JsonDocument.Parse(content);

            return await SteamDataMapper.MapOwnedGamesAsync(
                jsonDocument,
                steamId,
                async (id, appId) => await GetPlayerAchievementsAsync(id, appId, cancellationToken)
            );
        }
        catch (OperationCanceledException ex)
        {
            throw new SteamApiException("Requisição cancelada ou tempo limite atingido ao buscar jogos do usuário.", ex);
        }
        catch (Exception ex)
        {
            throw new SteamApiException("Erro inesperado ao buscar jogos do usuário.", ex);
        }
    }


    public async Task<List<Achievement>> GetPlayerAchievementsAsync(string steamId, int appId, CancellationToken cancellationToken = default)
    {
        var url = $"http://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key={_apiKey}&steamid={steamId}&appid={appId}";

        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return new List<Achievement>();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var jsonDocument = JsonDocument.Parse(content);
            return SteamDataMapper.MapPlayerAchievements(jsonDocument);
        }
        catch (OperationCanceledException ex)
        {
            throw new SteamApiException("Requisição cancelada ou tempo limite atingido ao buscar conquistas do jogador.", ex);
        }
        catch (Exception ex)
        {
            throw new SteamApiException("Erro inesperado ao buscar conquistas do jogador.", ex);
        }
    }
}