using Microsoft.EntityFrameworkCore;

namespace GranTurismoDataConsole.GranTurismoDataDbContext
{
    public class GTDataDbContext : DbContext
    {
        public GTDataDbContext(DbContextOptions<GTDataDbContext> options) : base(options)
        {

        }

        public DbSet<Link> Links { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost,1433;Database=GranTurismoData;User=sa;Password=Password1*;TrustServerCertificate=True");
        }
    }
}
