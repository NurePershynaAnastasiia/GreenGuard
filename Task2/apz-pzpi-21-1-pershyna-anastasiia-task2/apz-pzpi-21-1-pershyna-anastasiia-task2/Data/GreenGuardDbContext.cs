using apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database;
using Microsoft.EntityFrameworkCore;
using Task = apz_pzpi_21_1_pershyna_anastasiia_task2.Models.Database.Task;

namespace apz_pzpi_21_1_pershyna_anastasiia_task2.Data
{
    public class GreenGuardDbContext : DbContext
    {
        public GreenGuardDbContext(DbContextOptions<GreenGuardDbContext> options) : base(options) { }

        public DbSet<Plant> Plants { get; set; }

        public DbSet<Worker> Workers { get; set; }

        public DbSet<Fertilizer> Fertilizers { get; set; }

        public DbSet<Pest> Pests { get; set; }

        public DbSet<PlantType> PlantTypes { get; set; }

        public DbSet<Reminder> Reminders { get; set; }

        public DbSet<Task> Tasks { get; set; }
    }
