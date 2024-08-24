using Microsoft.EntityFrameworkCore;

namespace GranTurismoApi.GranTurismoApiDbContext
{
    public class GranTurismoDbContext : DbContext
    {
        public GranTurismoDbContext(DbContextOptions<GranTurismoDbContext> options) : base(options)
        {

        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Circuit> Circuits { get; set; }
    }
}
