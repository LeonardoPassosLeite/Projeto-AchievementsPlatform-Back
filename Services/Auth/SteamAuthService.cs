using System.Text.RegularExpressions;
using AchievementsPlatform.Exceptions;
using AchievementsPlatform.Helpers;
using AchievementsPlatform.Services.Auth.Interfaces;
using AchievementsPlatform.Services.Interfaces;

namespace AchievementsPlatform.Services
{
    public class SteamAuthService : ISteamAuthService
    {
        private readonly string _openIdNamespace;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SteamAuthService> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICookieService _cookieService;

        public SteamAuthService(
            IConfiguration configuration,
            ILogger<SteamAuthService> logger,
            IAuthenticationService authenticationService,
            ICookieService cookieService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _cookieService = cookieService ?? throw new ArgumentNullException(nameof(cookieService));
            _openIdNamespace = ConfigurationHelper.GetConfigurationValue(_configuration, "Steam:OpenIdNamespace", _logger);
        }

        public string GetSteamLoginUrl()
        {
            var redirectUrl = ConfigurationHelper.GetConfigurationValue(_configuration, "Steam:CallbackUrl", _logger);
            var realmUrl = ConfigurationHelper.GetConfigurationValue(_configuration, "Steam:RealmUrl", _logger);

            var queryString = new QueryStringBuilder()
                .Add("openid.ns", _openIdNamespace)
                .Add("openid.mode", "checkid_setup")
                .Add("openid.return_to", redirectUrl)
                .Add("openid.realm", realmUrl)
                .Add("openid.identity", $"{_openIdNamespace}/identifier_select")
                .Add("openid.claimed_id", $"{_openIdNamespace}/identifier_select")
                .Build();

            var steamLoginUrl = $"https://steamcommunity.com/openid/login?{queryString}";

            if (!Uri.IsWellFormedUriString(steamLoginUrl, UriKind.Absolute))
            {
                _logger.LogError("URL de login do Steam construída é inválida.");
                throw new InvalidOperationException("URL de login do Steam é inválida.");
            }

            _logger.LogInformation($"URL de login do Steam gerada com sucesso: {steamLoginUrl}");
            return steamLoginUrl;
        }

        public SteamAuthResult HandleCallback(IQueryCollection queryString, HttpResponse response)
        {
            ValidateQueryParameters(queryString);

            var claimedId = queryString["openid.claimed_id"].FirstOrDefault();
            if (string.IsNullOrEmpty(claimedId))
            {
                _logger.LogWarning("Parâmetro 'openid.claimed_id' está ausente ou vazio.");
                throw new SteamAuthException("Parâmetro 'openid.claimed_id' é inválido ou ausente.");
            }

            var steamId = ExtractSteamId(claimedId);
            var expiration = TimeSpan.Parse(ConfigurationHelper.GetConfigurationValue(_configuration, "Jwt:TokenExpiration", _logger));
            var token = _authenticationService.GenerateToken(steamId, "Usuario", expiration);

            _cookieService.SetJwtCookie(response, token, expiration);

            _logger.LogInformation("Callback do Steam processado com sucesso.");
            return new SteamAuthResult(ConfigurationHelper.GetConfigurationValue(_configuration, "Frontend:CallbackUrl", _logger), steamId);
        }

        private void ValidateQueryParameters(IQueryCollection queryString)
        {
            if (!queryString.ContainsKey("openid.mode") || queryString["openid.mode"] != "id_res")
            {
                _logger.LogWarning("Callback falhou: openid.mode ausente ou inválido.");
                throw new SteamAuthException("Parâmetros inválidos: openid.mode ausente ou inválido.");
            }

            if (!queryString.ContainsKey("openid.claimed_id") || string.IsNullOrEmpty(queryString["openid.claimed_id"]))
            {
                _logger.LogWarning("Callback falhou: claimed_id ausente ou inválido.");
                throw new SteamAuthException("Parâmetros inválidos: claimed_id ausente ou inválido.");
            }
        }

        private string ExtractSteamId(string claimedId)
        {
            if (!Uri.IsWellFormedUriString(claimedId, UriKind.Absolute))
            {
                _logger.LogWarning("Callback falhou: claimed_id inválido.");
                throw new SteamAuthException("Erro ao autenticar com a Steam: claimed_id inválido.");
            }

            var match = Regex.Match(claimedId, @"\d{17}$");
            return match.Success ? match.Value : throw new SteamAuthException("Steam ID não foi extraído corretamente.");
        }
    }
}