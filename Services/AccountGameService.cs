using AchievementsPlatform.Dtos;
using AchievementsPlatform.Repositories.Interfaces;
using AchievementsPlatform.Services.Interfaces;
using AutoMapper;

namespace AchievementsPlatform.Services
{
    public class AccountGameService : IAccountGameService
    {
        private readonly IAccountGameRepository _accountGameRepository;
        private readonly ISteamService _steamService;
        private readonly ILogger<AccountGameService> _logger;
        private readonly IMapper _mapper;  
        public AccountGameService(
            IAccountGameRepository accountGameRepository,
            ISteamService steamService,
            ILogger<AccountGameService> logger,
            IMapper mapper) 
        {
            _accountGameRepository = accountGameRepository ?? throw new ArgumentNullException(nameof(accountGameRepository));
            _steamService = steamService ?? throw new ArgumentNullException(nameof(steamService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task StoreUserGamesAsync(string steamId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(steamId))
            {
                _logger.LogWarning("SteamId está vazio ou nulo.");
                throw new ArgumentException("SteamId inválido.", nameof(steamId));
            }

            _logger.LogInformation($"Iniciando o armazenamento de jogos para o SteamId: {steamId}");

            var ownedGames = await _steamService.GetOwnedGamesAsync(steamId, cancellationToken);

            if (!ownedGames.Any())
            {
                _logger.LogWarning($"Nenhum jogo encontrado para o SteamId: {steamId}");
                return;
            }

            foreach (var game in ownedGames)
            {
                var gameDto = _mapper.Map<AccountGameDto>(game);
                await _accountGameRepository.AddGameIfNotExists(gameDto, steamId);
            }

            _logger.LogInformation($"Jogos armazenados com sucesso para o SteamId: {steamId}");
        }

        public async Task<IEnumerable<AccountGameDto>> GetStoredGamesAsync(string steamId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(steamId))
            {
                _logger.LogWarning("SteamId está vazio ou nulo.");
                throw new ArgumentException("SteamId inválido.", nameof(steamId));
            }

            _logger.LogInformation($"Obtendo jogos armazenados para SteamId: {steamId}");

            var storedGames = await _accountGameRepository.GetGamesBySteamIdAsync(steamId);

            return _mapper.Map<IEnumerable<AccountGameDto>>(storedGames);
        }
    }
}