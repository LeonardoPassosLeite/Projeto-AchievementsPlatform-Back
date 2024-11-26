using AchievementsPlatform.Factories;
using AchievementsPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using AchievementsPlatform.Dtos;

namespace AchievementsPlatform.Repositories
{
    public class AccountGameRepository : IAccountGameRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AccountGameRepository> _logger;

        public AccountGameRepository(AppDbContext dbContext, ILogger<AccountGameRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AccountGame?> GetGameAsync(int appId, string steamId)
        {
            return await _dbContext.AccountGames
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.AppId == appId && g.SteamUserId == steamId);
        }

        public async Task<IEnumerable<AccountGame>> GetGamesBySteamIdAsync(string steamId)
        {
            return await _dbContext.AccountGames
                .AsNoTracking()
                .Where(g => g.SteamUserId == steamId)
                .ToListAsync();
        }

        public async Task AddGameIfNotExists(AccountGameDto gameDto, string steamId)
        {
            if (gameDto == null)
                throw new ArgumentNullException(nameof(gameDto));

            if (string.IsNullOrWhiteSpace(steamId))
                throw new ArgumentException("SteamId inválido.", nameof(steamId));

            var existingGame = await _dbContext.AccountGames
                .FirstOrDefaultAsync(g => g.AppId == gameDto.AppId && g.SteamUserId == steamId);

            if (existingGame != null)
            {
                _logger.LogInformation($"Jogo já existente no banco: {gameDto.GameName} (AppId: {gameDto.AppId})");
                return;
            }

            var accountGame = AccountGameFactory.Create(gameDto, steamId, "https://cdn.steam.com/icons/");
            _logger.LogInformation($"Adicionando jogo ao banco: {gameDto.GameName} (AppId: {gameDto.AppId})");

            await _dbContext.AccountGames.AddAsync(accountGame);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<AccountGame?> GetGameBySteamUserIdAndAppIdAsync(string steamUserId, int appId)
        {
            return await _dbContext.AccountGames
                .FirstOrDefaultAsync(a => a.SteamUserId == steamUserId && a.AppId == appId);
        }
    }
}