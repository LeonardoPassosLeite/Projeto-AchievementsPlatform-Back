public class AccountGame : Base
{
    public int AppId { get; set; }
    public int PlaytimeForever { get; set; }
    public string IconUrl { get; set; } = string.Empty;
    public string SteamUserId { get; set; } = string.Empty;
    public int? UserAchievements { get; set; }
    public int? TotalAchievements { get; set; }
    
}
