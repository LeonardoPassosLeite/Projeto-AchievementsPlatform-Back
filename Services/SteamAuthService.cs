using AchievementsPlatform.Models;
using AchievementsPlatform.Services.Interfaces;

namespace AchievementsPlatform.Services
{
    public class SteamAuthService : ISteamAuthService
    {
        private const string SteamIdPrefix = "https://steamcommunity.com/openid/id/";
        private const string OpenIdNamespace = "http://specs.openid.net/auth/2.0";
        private readonly IConfiguration _configuration;
        private readonly ILogger<SteamAuthService> _logger;

        public SteamAuthService(IConfiguration configuration, ILogger<SteamAuthService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private string GetEnvironmentVariable(string key)
        {
            return Environment.GetEnvironmentVariable(key)
                   ?? throw new InvalidOperationException($"A variável de ambiente '{key}' não está configurada.");
        }

        public string GetSteamLoginUrl()
        {
            var redirectUrl = _configuration["Steam:CallbackUrl"]
                ?? throw new InvalidOperationException("CallbackUrl não configurada no appsettings.");
            var realmUrl = _configuration["Steam:RealmUrl"]
                ?? throw new InvalidOperationException("RealmUrl não configurada no appsettings.");

            var queryString = new QueryStringBuilder()
                .Add("openid.ns", OpenIdNamespace)
                .Add("openid.mode", "checkid_setup")
                .Add("openid.return_to", redirectUrl)
                .Add("openid.realm", realmUrl)
                .Add("openid.identity", $"{OpenIdNamespace}/identifier_select")
                .Add("openid.claimed_id", $"{OpenIdNamespace}/identifier_select")
                .Build();

            return $"https://steamcommunity.com/openid/login?{queryString}";
        }

        public SteamAuthResult HandleCallback(IQueryCollection queryString)
        {
            if (!queryString.ContainsKey("openid.mode") || queryString["openid.mode"] != "id_res")
            {
                _logger.LogWarning("Callback falhou: openid.mode ausente ou inválido.");
                throw new SteamAuthException("Erro ao autenticar com a Steam: openid.mode ausente ou inválido.");
            }

            var claimedId = queryString["openid.claimed_id"];
            if (string.IsNullOrEmpty(claimedId) || !Uri.IsWellFormedUriString(claimedId, UriKind.Absolute))
            {
                _logger.LogWarning("Callback falhou: claimed_id ausente ou inválido.");
                throw new SteamAuthException("Erro ao autenticar com a Steam: claimed_id ausente ou inválido.");
            }

            var steamId = ExtractSteamIdFromClaimedId(claimedId);
            if (string.IsNullOrEmpty(steamId))
            {
                _logger.LogWarning("Callback falhou: Steam ID inválido extraído.");
                throw new SteamAuthException("Erro ao autenticar com a Steam: Steam ID inválido.");
            }

            var frontEndCallbackUrl = GetConfigurationValue("FrontEnd:CallbackUrl");
            var redirectUrl = new QueryStringBuilder()
                .Add("steamId", steamId)
                .Add("message", "Login realizado com sucesso!")
                .Build();

            return new SteamAuthResult($"{frontEndCallbackUrl}?{redirectUrl}", steamId);
        }
        private string ExtractSteamIdFromClaimedId(string claimedId)
        {
            return claimedId.StartsWith(SteamIdPrefix) ? claimedId.Substring(SteamIdPrefix.Length) : null;
        }

        private string GetConfigurationValue(string key)
        {
            return _configuration[key] ?? throw new InvalidOperationException($"A configuração {key} não está definida.");
        }
    }
}