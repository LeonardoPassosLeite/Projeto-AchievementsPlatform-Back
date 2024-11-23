using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SteamAchievementsController : ControllerBase
{
    private readonly SteamService _steamService;

    public SteamAchievementsController(SteamService steamService)
    {
        _steamService = steamService;
    }

    [HttpGet("{steamId}/achievements")]
    public async Task<IActionResult> GetUserAchievements(string steamId)
    {
        try
        {
            var result = await _steamService.GetUserAchievementsByGame(steamId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}