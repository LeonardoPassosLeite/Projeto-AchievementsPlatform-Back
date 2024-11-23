public class Player : Base
{
    public string SteamId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TotalGames { get; set; }
    public int TotalHoursPlayed { get; set; }
    public List<SteamAchievement> SteamAchievements { get; set; }
}


