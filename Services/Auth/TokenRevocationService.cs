using AchievementsPlatform.Services.Auth.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace AchievementsPlatform.Services.Auth
{
    public class TokenRevocationService : ITokenRevocationService
    {
        private readonly IDistributedCache _cache;

        public TokenRevocationService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task RevokeTokenAsync(string tokenId, TimeSpan expiration)
        {
            await _cache.SetStringAsync(tokenId, "revoked", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            });
        }

        public async Task<bool> IsTokenRevokedAsync(string tokenId)
        {
            var value = await _cache.GetStringAsync(tokenId);
            return value == "revoked";
        }
    }
}