using AchievementsPlatform.Entities;
using AchievementsPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AchievementsPlatform.Repositories
{
    public class GameFeedbackRepository : Repository<GameFeedback>, IGameFeedbackRepository
    {
        private readonly AppDbContext _dbContext;

        public GameFeedbackRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GameFeedback?> GetFeedbackByUserAndAppAsync(string steamUserId, int appId)
        {
            return await _dbContext.GameFeedbacks
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.SteamUserId == steamUserId && f.AppId == appId);
        }

        public async Task<List<GameFeedback>> GetFeedbacksByAppIdAsync(int appId)
        {
            return await _dbContext.GameFeedbacks
                .AsNoTracking()
                .Where(f => f.AppId == appId)
                .ToListAsync();
        }
    }
}