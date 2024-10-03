using GranTurismoDataConsole;
using Microsoft.EntityFrameworkCore;

public class GTDataDbContext : DbContext
{
    public DbSet<Link> Links { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=localhost,1433;Database=GranTurismoData;User=sa;Password=Password1*;TrustServerCertificate=True");
    }
}