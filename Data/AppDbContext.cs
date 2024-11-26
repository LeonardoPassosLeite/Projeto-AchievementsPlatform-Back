using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<AccountGame> AccountGames { get; set; }
    

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

}
