using AchievementsPlatform.Exceptions;
using AchievementsPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly ISteamAuthService _steamAuthService;
    private readonly IAccountGameService _accountGameService;
    private readonly ITokenService _tokenService;

    public LoginController(ILogger<LoginController> logger,
                           ISteamAuthService steamAuthService,
                           IAccountGameService accountGameService,
                           ITokenService tokenService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _steamAuthService = steamAuthService ?? throw new ArgumentNullException(nameof(steamAuthService));
        _accountGameService = accountGameService ?? throw new ArgumentNullException(nameof(accountGameService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    [HttpGet("steam")]
    public IActionResult LoginWithSteam()
    {
        try
        {
            var steamLoginUrl = _steamAuthService.GetSteamLoginUrl();
            return Redirect(steamLoginUrl);
        }
        catch (Exception)
        {
            return StatusCode(500, "Erro interno ao redirecionar para o Steam.");
        }
    }

    [HttpGet("auth/callback")]
    public IActionResult AuthCallback()
    {
        try
        {
            var result = _steamAuthService.HandleCallback(HttpContext.Request.Query);
            var token = _tokenService.GenerateToken(result.SteamId, result.Username);

            Response.Cookies.Append("jwtToken", token, new CookieOptions
            {
                HttpOnly = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production",
                Secure = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production",
                SameSite = SameSiteMode.Strict
            });

            return Redirect("http://localhost:4200/auth/callback");
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
    [Authorize]
    public async Task<IActionResult> StoreAccountGameUserData()
    {
        var steamId = User.Claims.FirstOrDefault(c => c.Type == "steamId")?.Value;

        if (string.IsNullOrEmpty(steamId))
            return BadRequest(new { error = "SteamId não encontrado no token." });

        try
        {
            await _accountGameService.StoreUserGamesAsync(steamId);
            return Ok(new { message = "Jogos armazenados com sucesso." });
        }
        catch (UserDataException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (DbUpdateException)
        {
            return StatusCode(500, new { error = "Erro ao salvar dados no banco de dados." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { error = "Erro interno ao processar a solicitação." });
        }
    }
}