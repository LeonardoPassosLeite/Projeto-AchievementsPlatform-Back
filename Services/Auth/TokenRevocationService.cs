using AchievementsPlatform.Services.Auth.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace AchievementsPlatform.Services.Auth
{
    public class TokenRevocationService : ITokenRevocationService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<TokenRevocationService> _logger;

        public TokenRevocationService(IDistributedCache cache, ILogger<TokenRevocationService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task RevokeTokenAsync(string tokenId, TimeSpan expiration)
        {
            _logger.LogInformation("Revoking token with ID: {TokenId}, Expiration: {Expiration}", tokenId, expiration);

            await _cache.SetStringAsync(tokenId, "revoked", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            });

            _logger.LogInformation("Token with ID: {TokenId} revoked successfully", tokenId);
        }

        public async Task<bool> IsTokenRevokedAsync(string tokenId)
        {
            _logger.LogInformation("Checking if token with ID: {TokenId} is revoked", tokenId);

            var value = await _cache.GetStringAsync(tokenId);

            if (value == "revoked")
            {
                _logger.LogWarning("Token with ID: {TokenId} is revoked", tokenId);
                return true;
            }

            _logger.LogInformation("Token with ID: {TokenId} is not revoked", tokenId);
            return false;
        }
    }
}