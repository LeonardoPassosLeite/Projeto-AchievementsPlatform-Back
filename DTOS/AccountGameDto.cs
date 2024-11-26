namespace AchievementsPlatform.Dtos
{
    public class AccountGameDto
    {
        public int AppId { get; set; }
        public string GameName { get; set; } = string.Empty;
        public int PlaytimeForever { get; set; }
        public int AchievementsCount { get; set; }
        public int TotalAchievements { get; set; }
    }
}

