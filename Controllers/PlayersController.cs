using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly AppDbContext _context;

    public PlayersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetPlayers()
    {
        var players = _context.Players.Include(p => p.SteamAchievements).ToList();
        return Ok(players);
    }

    [HttpPost]
    public IActionResult AddPlayer(Player player)
    {
        _context.Players.Add(player);
        _context.SaveChanges();
        return CreatedAtAction(nameof(GetPlayers), new { id = player.Id }, player);
    }
}