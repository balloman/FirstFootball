namespace FirstFootball.Backend.Models;

public class BackendContext : DbContext
{
    public DbSet<Fixture> Fixtures => Set<Fixture>();
    public DbSet<Team> Teams => Set<Team>();
    
    public BackendContext(){}
    
    public BackendContext(DbContextOptions<BackendContext> options) : base(options)
    {
    }
}
