using AchievementsPlatform.Models;
using AchievementsPlatform.Services;
using AchievementsPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly ISteamAuthService _steamAuthService;
    private readonly ISteamService _steamService;
    private readonly IAccountGameService _accountGameService;
    private readonly AppDbContext _dbContext;

    public LoginController(ILogger<LoginController> logger,
                           ISteamAuthService steamAuthService,
                           ISteamService steamService,
                           IAccountGameService accountGameService,
                           AppDbContext dbContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _steamAuthService = steamAuthService ?? throw new ArgumentNullException(nameof(steamAuthService));
        _steamService = steamService ?? throw new ArgumentNullException(nameof(steamService));
        _accountGameService = accountGameService ?? throw new ArgumentNullException(nameof(accountGameService));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    [HttpGet("steam")]
    public IActionResult LoginWithSteam()
    {
        try
        {
            var steamLoginUrl = _steamAuthService.GetSteamLoginUrl();
            return Redirect(steamLoginUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao construir a URL de login do Steam.");
            return StatusCode(500, "Erro interno ao redirecionar para o Steam.");
        }
    }

    [HttpGet("auth/callback")]
    public IActionResult AuthCallback()
    {
        try
        {
            var result = _steamAuthService.HandleCallback(HttpContext.Request.Query);

            _logger.LogInformation($"Usuário autenticado com sucesso. SteamId: {result.SteamId}");

            return Redirect(result.RedirectUrl);
        }
        catch (SteamAuthException ex)
        {
            _logger.LogWarning(ex, "Erro de autenticação com a Steam.");
            return Redirect($"http://localhost:4200/auth/callback?message={Uri.EscapeDataString(ex.Message)}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado durante o callback.");
            return Redirect($"http://localhost:4200/auth/callback?message=Erro interno durante a autenticação.");
        }
    }

    [HttpPost("store-accountgame-user-data")]
    public async Task<IActionResult> StoreUserData([FromBody] StoreUserDTO storeUserDTO)
    {
        if (string.IsNullOrWhiteSpace(storeUserDTO.SteamId))
            return BadRequest(new { message = "SteamId não fornecido." });

        try
        {
            await _accountGameService.StoreUserGamesAsync(storeUserDTO.SteamId);
            return Ok(new { message = "Dados armazenados com sucesso." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao armazenar dados do usuário.");
            return StatusCode(500, new { message = "Erro interno.", error = ex.Message });
        }
    }
}