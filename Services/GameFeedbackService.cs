using AchievementsPlatform.Dtos;
using AchievementsPlatform.Entities;
using AchievementsPlatform.Exceptions;
using AchievementsPlatform.Repositories.Interfaces;
using AchievementsPlatform.Services.Interfaces;
using AutoMapper;

public class GameFeedbackService : IGameFeedbackService
{
    private readonly IGameFeedbackRepository _gameFeedbackRepository;
    private readonly IAccountGameRepository _accountGameRepository;
    private readonly IMapper _mapper;

    public GameFeedbackService(
        IGameFeedbackRepository gameFeedbackRepository,
        IAccountGameRepository accountGameRepository,
        IMapper mapper)
    {
        _gameFeedbackRepository = gameFeedbackRepository ?? throw new ArgumentNullException(nameof(gameFeedbackRepository));
        _accountGameRepository = accountGameRepository ?? throw new ArgumentNullException(nameof(accountGameRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task AddOrUpdateFeedbackAsync(string steamUserId, int appId, string comment, int rating, bool recommend)
    {
        try
        {
            var ownedGame = await _accountGameRepository.GetGameBySteamUserIdAndAppIdAsync(steamUserId, appId);
            if (ownedGame == null)
                throw new FeedbackException("Você não pode avaliar um jogo que não possui.");

            if (ownedGame.PlaytimeForever < 120)
                throw new FeedbackException("Você precisa ter pelo menos 2 horas de jogo para avaliá-lo.");

            var existingFeedback = await _gameFeedbackRepository.GetFeedbackByUserAndAppAsync(steamUserId, appId);
            if (existingFeedback != null)
                throw new FeedbackException("Você já avaliou e comentou este jogo. Edite o feedback existente se necessário.");

            var newFeedback = _mapper.Map<GameFeedback>(new GameFeedbackDto
            {
                SteamUserId = steamUserId,
                AppId = appId,
                Comment = comment,
                Rating = rating,
                Recommend = recommend,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            newFeedback.ValidateFeedbackInput(steamUserId, appId, rating);
            await _gameFeedbackRepository.AddAsync(newFeedback);
        }
        catch (FeedbackException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new FeedbackException("Ocorreu um erro ao adicionar o feedback. Tente novamente mais tarde.");
        }
    }

    public async Task<IEnumerable<GameFeedbackDto>> GetFeedbackByGameAsync(int appId)
    {
        if (appId <= 0)
            throw new ArgumentException("O AppId deve ser um valor válido.", nameof(appId));
        try
        {
            var feedbacks = await _gameFeedbackRepository.GetFeedbacksByAppIdAsync(appId);

            if (!feedbacks.Any())
                return Enumerable.Empty<GameFeedbackDto>();

            return _mapper.Map<IEnumerable<GameFeedbackDto>>(feedbacks);
        }
        catch (Exception)
        {
            throw new FeedbackException("Erro ao buscar feedbacks. Tente novamente mais tarde.");
        }
    }

    public async Task UpdateFeedbackAsync(string steamUserId, int appId, string comment, int rating, bool recommend)
    {
        try
        {
            var existingFeedback = await _gameFeedbackRepository.GetFeedbackByUserAndAppAsync(steamUserId, appId);

            if (existingFeedback == null)
                throw new FeedbackException("Nenhum feedback encontrado para este jogo. Adicione um feedback primeiro.");

            existingFeedback.Update(steamUserId, appId, comment, rating, recommend);

            await _gameFeedbackRepository.UpdateAsync(existingFeedback);
        }
        catch (FeedbackException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new FeedbackException("Ocorreu um erro ao atualizar o feedback. Tente novamente mais tarde.");
        }
    }

    public async Task DeleteFeedbackAsync(string steamUserId, int appId)
    {

        var feedback = await _gameFeedbackRepository.GetFeedbackByUserAndAppAsync(steamUserId, appId);
        if (feedback == null)
            throw new FeedbackException("Nenhum feedback encontrado para remover.");

        try
        {
            await _gameFeedbackRepository.DeleteAsync(feedback);
        }
        catch (Exception)
        {
            throw new FeedbackException("Erro ao remover feedback. Tente novamente.");
        }
    }
}