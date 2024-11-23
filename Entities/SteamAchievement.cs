public class SteamAchievement : Base
{
    public string ApiName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Achieved { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string SteamId { get; set; } = string.Empty;
    public DateTime? UnlockDate { get; set; }
}
