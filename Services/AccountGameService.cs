using AchievementsPlatform.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AchievementsPlatform.Services
{
    public class AccountGameService : IAccountGameService
    {
        private readonly AppDbContext _dbContext;
        private readonly ISteamService _steamService;
        private readonly ILogger<AccountGameService> _logger;

        public AccountGameService(AppDbContext dbContext, ISteamService steamService, ILogger<AccountGameService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _steamService = steamService ?? throw new ArgumentNullException(nameof(steamService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StoreUserGamesAsync(string steamId)
        {
            if (string.IsNullOrWhiteSpace(steamId))
            {
                _logger.LogWarning("SteamId está vazio ou nulo.");
                throw new ArgumentException("SteamId inválido.", nameof(steamId));
            }

            _logger.LogInformation($"Iniciando o processo de armazenamento de jogos para SteamId: {steamId}");

            try
            {
                // Obtém os jogos do usuário via SteamService
                var games = await _steamService.GetUserAchievementsByGame(steamId);
                if (games == null || !games.Any())
                {
                    _logger.LogWarning($"Nenhum jogo encontrado para o SteamId: {steamId}");
                    return;
                }

                foreach (var game in games)
                {
                    var existingGame = await _dbContext.AccountGames
                        .FirstOrDefaultAsync(g => g.AppId == game.AppId && g.SteamUserId == steamId);

                    if (existingGame == null)
                    {
                        var newGame = new AccountGame
                        {
                            AppId = game.AppId,
                            Name = game.GameName,
                            PlaytimeForever = game.PlaytimeForever,
                            IconUrl = $"https://cdn.steam.com/icons/{game.AppId}",
                            SteamUserId = steamId,
                            UserAchievements = game.Achievements.Count(a => a.Achieved),
                            TotalAchievements = game.Achievements.Count
                        };

                        _logger.LogInformation($"Adicionando jogo ao banco: {game.GameName} (AppId: {game.AppId})");
                        await _dbContext.AccountGames.AddAsync(newGame);
                    }
                    else
                    {
                        _logger.LogInformation($"Jogo já existe no banco: {game.GameName} (AppId: {game.AppId})");
                    }
                }

                _logger.LogInformation("Salvando alterações no banco de dados...");
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Jogos armazenados com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao armazenar jogos para o SteamId: {steamId}");
                throw;
            }
        }
    }
}