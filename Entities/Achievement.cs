public class Achievement
{
    public string ApiName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool Achieved { get; set; }
}
public class GameWithAchievements
{
    public string GameName { get; set; } = string.Empty;
    public int AppId { get; set; }
    public int PlaytimeForever { get; set; }
    public List<Achievement> Achievements { get; set; }
}