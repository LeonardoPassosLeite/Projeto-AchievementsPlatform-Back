using AchievementsPlatform.Exceptions;
using AchievementsPlatform.Services.Auth.Interfaces;
using AchievementsPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly ISteamAuthService _steamAuthService;
    private readonly ICookieService _coockieService;
    private readonly IAuthenticationService _authenticationService;
    private readonly ITokenRevocationService _tokenRevocationService;

    public LoginController(ILogger<LoginController> logger,
                           ISteamAuthService steamAuthService,
                           IAccountGameService accountGameService,
                           ICookieService coockieService,
                           IAuthenticationService authenticationService,
                           ITokenRevocationService tokenRevocationService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _steamAuthService = steamAuthService ?? throw new ArgumentNullException(nameof(steamAuthService));
        _coockieService = coockieService ?? throw new ArgumentNullException(nameof(coockieService));
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _tokenRevocationService = tokenRevocationService ?? throw new ArgumentNullException(nameof(tokenRevocationService));
    }

    [HttpGet("steam")]
    public IActionResult LoginWithSteam()
    {
        try
        {
            var steamLoginUrl = _steamAuthService.GetSteamLoginUrl();
            return Redirect(steamLoginUrl);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Erro de configuração ao obter a URL de login do Steam.");
            return Problem(
                detail: "O sistema está mal configurado para redirecionar ao Steam. Entre em contato com o suporte.",
                title: "Erro de Configuração Interna",
                statusCode: 500
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao redirecionar para o Steam.");
            return Problem(
                detail: "Ocorreu um erro inesperado ao processar sua solicitação.",
                title: "Erro Interno",
                statusCode: 500
            );
        }
    }

    [HttpGet("auth/callback")]
    public IActionResult AuthCallback()
    {
        try
        {
            var result = _steamAuthService.HandleCallback(HttpContext.Request.Query, Response);
            return Redirect(result.CallbackUrl);
        }
        catch (SteamAuthException ex)
        {
            _logger.LogWarning(ex, "Erro de autenticação com a Steam.");
            return Problem(
                detail: "Falha na autenticação com a Steam. Verifique as credenciais e tente novamente.",
                title: "Erro de Autenticação",
                statusCode: 401
            );
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Parâmetro inválido recebido.");
            return Problem(
                detail: "Parâmetros inválidos foram fornecidos na requisição.",
                title: "Erro de Parâmetros",
                statusCode: 400
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado durante o callback.");
            return Problem(
                detail: "Ocorreu um erro inesperado durante o processamento da autenticação.",
                title: "Erro Interno",
                statusCode: 500
            );
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var jwtToken = Request.Cookies["jwtToken"];
            if (string.IsNullOrEmpty(jwtToken))
            {
                _logger.LogWarning("Nenhum cookie JWT encontrado durante o logout.");
                return BadRequest(new { message = "Nenhum token encontrado para realizar o logout." });
            }

            var tokenId = _authenticationService.GetTokenId(jwtToken);
            if (string.IsNullOrEmpty(tokenId))
            {
                _logger.LogWarning("Falha ao extrair o ID do token durante o logout.");
                return BadRequest(new { message = "Token inválido ou malformado." });
            }

            var expiration = _authenticationService.GetTokenExpiration(jwtToken);
            await _tokenRevocationService.RevokeTokenAsync(tokenId, expiration);

            _coockieService.ClearJwtCookie(Response);

            _logger.LogInformation($"Logout realizado com sucesso. Token ID {tokenId} revogado.");
            return Ok(new { message = "Logout realizado com sucesso." });
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "Erro ao processar o token durante o logout.");
            return Problem(
                detail: "Token inválido ou malformado. Não foi possível processar o logout.",
                title: "Erro de Token",
                statusCode: 400
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao realizar o logout.");
            return Problem(
                detail: "Ocorreu um erro inesperado ao processar o logout.",
                title: "Erro Interno",
                statusCode: 500
            );
        }
    }
}