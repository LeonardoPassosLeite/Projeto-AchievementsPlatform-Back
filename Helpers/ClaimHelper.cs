using System.Security.Claims;

namespace AchievementsPlatform.Helpers
{
    public static class ClaimHelper
    {
        public static string GetClaimValue(IEnumerable<Claim> claims, string claimType)
        {
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            return claims.FirstOrDefault(c => c.Type == claimType)?.Value
                ?? throw new InvalidOperationException($"Claim '{claimType}' não encontrada.");
        }

        public static bool HasClaim(IEnumerable<Claim> claims, string claimType)
        {
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            return claims.Any(c => c.Type == claimType);
        }
    }
}