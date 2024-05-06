using GreenGuard.Models.Database;
using Microsoft.EntityFrameworkCore;
using Task = GreenGuard.Models.Database.Task;

namespace GreenGuard.Data
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

        public DbSet<WorkerInTask> Worker_in_Task { get; set; }

        public DbSet<PlantInTask> Plant_in_Task { get; set; }
    }
}
