using apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database;
using Microsoft.EntityFrameworkCore;
using Task = apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database.Task;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Data
{
    public class GreenGuardDbContext : DbContext
    {
        public GreenGuardDbContext(DbContextOptions<GreenGuardDbContext> options) : base(options) { }

        public DbSet<Plant> Plant { get; set; }

        public DbSet<Worker> Worker { get; set; }

        public DbSet<Fertilizer> Fertilizer { get; set; }

        public DbSet<Pest> Pest { get; set; }

        public DbSet<PlantType> Plant_type { get; set; }

        public DbSet<Task> Task { get; set; }
    }
}
