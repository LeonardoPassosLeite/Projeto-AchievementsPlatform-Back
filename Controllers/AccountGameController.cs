using AchievementsPlatform.Helpers;
using AchievementsPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AchievementsPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountGameController : ControllerBase
    {
        private readonly ILogger<AccountGameController> _logger;
        private readonly IAccountGameService _accountGameService;
        public AccountGameController(ILogger<AccountGameController> logger,
                                     IAccountGameService accountGameService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _accountGameService = accountGameService ?? throw new ArgumentNullException(nameof(accountGameService));
        }

        [HttpPost("store-games")]
        [Authorize]
        public async Task<IActionResult> StoreAccountGames()
        {
            try
            {
                var steamId = ClaimHelper.GetClaimValue(User.Claims, "steamId");
                if (string.IsNullOrEmpty(steamId))
                {
                    _logger.LogWarning("SteamId não encontrado nos claims do usuário.");
                    return Problem(
                        detail: "SteamId não encontrado. Certifique-se de que o token de autenticação está correto.",
                        title: "Erro de Validação",
                        statusCode: 400
                    );
                }

                await _accountGameService.StoreUserGamesAsync(steamId);

                _logger.LogInformation($"Jogos armazenados com sucesso para o SteamId: {steamId}");
                return Ok(new { message = "Jogos armazenados com sucesso." });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro ao armazenar jogos.");
                return Problem(
                    detail: ex.Message,
                    title: "Erro de Validação",
                    statusCode: 400
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao armazenar jogos.");
                return Problem(
                    detail: "Ocorreu um erro inesperado ao processar o armazenamento de jogos.",
                    title: "Erro Interno",
                    statusCode: 500
                );
            }
        }

        [HttpGet("stored-games")]
        [Authorize]
        public async Task<IActionResult> GetStoredGames()
        {
            try
            {
                var steamId = ClaimHelper.GetClaimValue(User.Claims, "steamId");
                if (string.IsNullOrEmpty(steamId))
                {
                    _logger.LogWarning("SteamId não encontrado nos claims do usuário.");
                    return Problem(
                        detail: "SteamId não encontrado. Certifique-se de que o token de autenticação está correto.",
                        title: "Erro de Validação",
                        statusCode: 400
                    );
                }

                var games = await _accountGameService.GetStoredGamesAsync(steamId);

                _logger.LogInformation($"Jogos armazenados obtidos com sucesso para o SteamId: {steamId}");
                return Ok(games);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro ao obter jogos armazenados.");
                return Problem(
                    detail: ex.Message,
                    title: "Erro de Validação",
                    statusCode: 400
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao obter jogos armazenados.");
                return Problem(
                    detail: "Ocorreu um erro inesperado ao processar a solicitação para obter os jogos armazenados.",
                    title: "Erro Interno",
                    statusCode: 500
                );
            }
        }
    }
}