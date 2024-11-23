namespace AchievementsPlatform.Services
{
    public class QueryStringBuilder
    {
        private readonly Dictionary<string, string> _parameters = new();

        public QueryStringBuilder Add(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            _parameters[key] = value ?? throw new ArgumentNullException(nameof(value));
            return this;
        }

        public string Build()
        {
            return string.Join("&", _parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
        }
    }
}