namespace AchievementsPlatform.Dtos
{
    public class GameFeedbackDto
    {
        public int Id { get; set; }
        public string SteamUserId { get; set; } = string.Empty;
        public int AppId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public bool Recommend { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}