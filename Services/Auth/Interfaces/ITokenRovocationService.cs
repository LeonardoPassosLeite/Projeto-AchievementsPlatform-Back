namespace AchievementsPlatform.Services.Auth.Interfaces
{
    public interface ITokenRevocationService
    {
        Task RevokeTokenAsync(string tokenId, TimeSpan expiration);
        Task<bool> IsTokenRevokedAsync(string tokenId);
    }
}