using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Player> Players { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Achievement> Achievements { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}