using AchievementsPlatform.Services.Auth.Interfaces;
using AchievementsPlatform.Services.Interfaces;

namespace AchievementsPlatform.Services.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(ITokenService tokenService, ILogger<AuthenticationService> logger)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string GenerateToken(string userId, string username, TimeSpan expiration)
        {
            _logger.LogInformation($"Gerando token para o usuário {userId}.");
            return _tokenService.GenerateToken(userId, username, expiration);
        }

        public string GetTokenId(string token)
        {
            return _tokenService.GetTokenId(token);
        }

        public TimeSpan GetTokenExpiration(string token)
        {
            return _tokenService.GetTokenExpiration(token);
        }
    }
}