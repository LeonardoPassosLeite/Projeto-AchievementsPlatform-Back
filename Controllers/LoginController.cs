using AchievementsPlatform.Models;
using AchievementsPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly ISteamAuthService _steamAuthService;

    public LoginController(ILogger<LoginController> logger, ISteamAuthService steamAuthService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _steamAuthService = steamAuthService ?? throw new ArgumentNullException(nameof(steamAuthService));
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
}