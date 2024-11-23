namespace AchievementsPlatform.Helpers
{
    public static class UriHelper
    {
        public static string BuildUri(string baseUri, IDictionary<string, string> queryParams)
        {
            if (string.IsNullOrWhiteSpace(baseUri))
                throw new ArgumentNullException(nameof(baseUri), "Base URI não pode ser nula ou vazia.");

            if (queryParams == null || !queryParams.Any())
                return baseUri;

            var queryString = string.Join("&", queryParams.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
            return $"{baseUri}{(baseUri.Contains("?") ? "&" : "?")}{queryString}";
        }

        public static bool IsValidUri(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.Absolute);
        }
    }
}