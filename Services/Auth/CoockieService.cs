using AchievementsPlatform.Services.Auth.Interfaces;

namespace AchievementsPlatform.Services
{
    public class CoockieService : ICookieService
    {
        private readonly bool _isProduction;

        public CoockieService()
        {
            _isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production";
        }

        public void SetJwtCookie(HttpResponse response, string token, TimeSpan expiration)
        {
            response.Cookies.Append("jwtToken", token, new CookieOptions
            {
                HttpOnly = _isProduction,
                Secure = _isProduction,
                SameSite = _isProduction ? SameSiteMode.Strict : SameSiteMode.Lax,
                Expires = DateTime.UtcNow.Add(expiration)
            });
        }

        public void ClearJwtCookie(HttpResponse response)
        {
            response.Cookies.Append("jwtToken", string.Empty, new CookieOptions
            {
                HttpOnly = _isProduction,
                Secure = _isProduction,
                SameSite = _isProduction ? SameSiteMode.Strict : SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(-1)
            });
        }
    }
}