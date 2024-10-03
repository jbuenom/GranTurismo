using GranTurismoDataConsole;
using Microsoft.EntityFrameworkCore;

public class GTDataDbContext : DbContext
{
    public DbSet<Link> Links { get; set; }

    // Este método se utiliza para configurar la base de datos.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Conexión de ejemplo para SQL Server (reemplázala según sea necesario)
        optionsBuilder.UseSqlServer(@"Server=localhost,1433;Database=GranTurismoData;User=sa;Password=Password1*;TrustServerCertificate=True");
    }
}