using AchievementsPlatform.Dtos;
using AchievementsPlatform.Exceptions;
using AchievementsPlatform.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AchievementsPlatform.Controllers
{
    [ApiController]
    [Route("api/feedback")]
    public class GameFeedbackController : ControllerBase
    {
        private readonly IGameFeedbackService _service;

        public GameFeedbackController(IGameFeedbackService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddFeedback([FromBody] GameFeedbackDto feedbackDto)
        {
            try
            {
                await _service.AddOrUpdateFeedbackAsync(feedbackDto.SteamUserId, feedbackDto.AppId, feedbackDto.Comment, feedbackDto.Rating, feedbackDto.Recommend);
                return Ok("Feedback salvo com sucesso.");
            }
            catch (FeedbackException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro inesperado ao salvar feedback.", details = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFeedback([FromBody] GameFeedbackDto feedbackDto)
        {
            await _service.UpdateFeedbackAsync(feedbackDto.SteamUserId, feedbackDto.AppId, feedbackDto.Comment, feedbackDto.Rating, feedbackDto.Recommend);
            return Ok("Feedback atualizado com sucesso.");
        }

        [HttpGet("{appId}")]
        public async Task<IActionResult> GetFeedback(int appId)
        {
            var feedback = await _service.GetFeedbackByGameAsync(appId);
            return Ok(feedback);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFeedback(string steamUserId, int appId)
        {
            await _service.DeleteFeedbackAsync(steamUserId, appId);
            return Ok("Feedback removido com sucesso.");
        }
    }
}