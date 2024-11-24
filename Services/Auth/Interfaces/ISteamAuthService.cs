namespace AchievementsPlatform.Services.Interfaces
{
    public interface ISteamAuthService
    {
        string GetSteamLoginUrl();
        SteamAuthResult HandleCallback(IQueryCollection queryString, HttpResponse response);
    }
}