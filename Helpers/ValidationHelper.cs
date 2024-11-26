using System.Text.RegularExpressions;

namespace AchievementsPlatform.Helpers
{
    public static class ValidationHelper
    {
        public static void ValidateUrl(string url, string errorMessage)
        {
            if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new ArgumentException(errorMessage);
        }

        public static void ValidateSteamId(string steamId)
        {
            if (string.IsNullOrEmpty(steamId) || !Regex.IsMatch(steamId, @"^\d{17}$"))
                throw new ArgumentException("Steam ID inválido.");
        }
    }
}