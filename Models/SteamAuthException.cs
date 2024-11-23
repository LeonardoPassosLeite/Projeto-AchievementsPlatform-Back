namespace AchievementsPlatform.Models
{
    public class SteamAuthException : System.Exception
    {
        public SteamAuthException(string message) : base(message)
        { }

        public SteamAuthException(string message, System.Exception innerException) : base(message, innerException)
        { }
    }
}