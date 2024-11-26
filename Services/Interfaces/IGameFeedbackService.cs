using AchievementsPlatform.Dtos;

namespace AchievementsPlatform.Services.Interfaces
{
    public interface IGameFeedbackService
    {
        Task AddOrUpdateFeedbackAsync(string steamUserId, int appId, string comment, int rating, bool recommend);
        Task<IEnumerable<GameFeedbackDto>> GetFeedbackByGameAsync(int appId);
        Task UpdateFeedbackAsync(string steamUserId, int appId, string comment, int rating, bool recommend); 
        Task DeleteFeedbackAsync(string steamUserId, int appId);
    }
}
