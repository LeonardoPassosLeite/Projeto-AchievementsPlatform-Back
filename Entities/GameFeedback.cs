namespace AchievementsPlatform.Entities
{
    public class GameFeedback
    {
        public int Id { get; set; }
        public string SteamUserId { get; set; } = string.Empty;
        public int AppId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public bool Recommend { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public void ValidateFeedbackInput(string steamUserId, int appId, int rating)
        {
            if (string.IsNullOrWhiteSpace(steamUserId))
                throw new ArgumentException("O SteamUserId não pode ser vazio.", nameof(steamUserId));

            if (appId <= 0)
                throw new ArgumentException("O AppId deve ser um valor válido.", nameof(appId));

            if (rating < 0 || rating > 10)
                throw new ArgumentException("A nota deve estar entre 0 e 10.", nameof(rating));
        }

        public void Update(string steamUserId, int appId, string comment, int rating, bool recommend)
        {
            ValidateFeedbackInput(steamUserId, appId, rating);

            Comment = comment;
            Rating = rating;
            Recommend = recommend;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}