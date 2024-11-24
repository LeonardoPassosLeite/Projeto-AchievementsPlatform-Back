using AchievementsPlatform.Exceptions;
using AchievementsPlatform.Services.Auth.Interfaces;
using AchievementsPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly ISteamAuthService _steamAuthService;
    private readonly IAccountGameService _accountGameService;
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
        _accountGameService = accountGameService ?? throw new ArgumentNullException(nameof(accountGameService));
        _coockieService = coockieService ?? throw new ArgumentNullException(nameof(coockieService));
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _tokenRevocationService = tokenRevocationService ?? throw new ArgumentNullException(nameof(tokenRevocationService));
    }

    [Authorize]
    [HttpGet("protected-resource")]
    public IActionResult GetProtectedResource()
    {
        var steamId = User.Claims.FirstOrDefault(c => c.Type == "steamId")?.Value;

        if (string.IsNullOrEmpty(steamId))
            return Unauthorized("Token inválido ou SteamId ausente.");

        return Ok(new { message = "Recurso protegido acessado com sucesso!" });
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

    [HttpPost("store-accountgame-user-data")]
    [Authorize]
    public async Task<IActionResult> StoreAccountGameUserData()
    {
        _logger.LogInformation($"Token recebido no header Authorization: {Request.Headers["Authorization"]}");

        var steamId = User.Claims.FirstOrDefault(c => c.Type == "steamId")?.Value;
        _logger.LogInformation($"SteamId extraído do token: {steamId}");

        if (string.IsNullOrEmpty(steamId))
        {
            _logger.LogWarning("SteamId não encontrado no token.");
            return BadRequest(new { error = "SteamId não encontrado no token." });
        }

        try
        {
            await _accountGameService.StoreUserGamesAsync(steamId);
            return Ok(new { message = "Jogos armazenados com sucesso." });
        }
        catch (UserDataException ex)
        {
            _logger.LogError(ex, "Erro ao armazenar dados do usuário.");
            return BadRequest(new { error = ex.Message });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Erro ao salvar dados no banco de dados.");
            return StatusCode(500, new { error = "Erro ao salvar dados no banco de dados." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao processar a solicitação.");
            return StatusCode(500, new { error = "Erro interno ao processar a solicitação." });
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