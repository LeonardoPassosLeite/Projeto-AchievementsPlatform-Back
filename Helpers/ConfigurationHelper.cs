namespace AchievementsPlatform.Helpers
{
    public static class ConfigurationHelper
    {
        public static string GetConfigurationValue(IConfiguration configuration, string key, ILogger logger = null)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var value = configuration[key];
            if (string.IsNullOrEmpty(value))
            {
                logger?.LogError($"Configuração ausente para a chave: {key}");
                throw new InvalidOperationException($"Configuração ausente: {key}");
            }

            return value;
        }
    }
}