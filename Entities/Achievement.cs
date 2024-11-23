public class Achievement
{
    public string ApiName { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Achieved { get; set; }
}

public class GameWithAchievements
{
    public string GameName { get; set; }
    public int AppId { get; set; }
    public List<Achievement> Achievements { get; set; }
}
