using AchievementsPlatform.Entities;

namespace AchievementsPlatform.Repositories.Interfaces
{
    public interface IGameFeedbackRepository : IRepository<GameFeedback>
    {
        Task<GameFeedback?> GetFeedbackByUserAndAppAsync(string steamUserId, int appId);
        Task<List<GameFeedback>> GetFeedbacksByAppIdAsync(int appId);
    }
}