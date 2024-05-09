using GreenGuard.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskDto = GreenGuard.Dto.TaskDto;

namespace GreenGuard.Data
{
    public class GreenGuardDbContext : DbContext
    {
        public GreenGuardDbContext(DbContextOptions<GreenGuardDbContext> options) : base(options) { }

        public DbSet<PlantDto> Plant { get; set; }

        public DbSet<WorkerDto> Worker { get; set; }

        public DbSet<FertilizerDto> Fertilizer { get; set; }

        public DbSet<PestDto> Pest { get; set; }

        public DbSet<PlantTypeDto> Plant_type { get; set; }

        public DbSet<TaskDto> Task { get; set; }

        public DbSet<WorkerInTaskDto> Worker_in_Task { get; set; }

        public DbSet<PlantInTaskDto> Plant_in_Task { get; set; }

        public DbSet<WorkingScheduleDto> Working_Schedule { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var timeOnlyConverter = new ValueConverter<TimeOnly, TimeSpan>(
                timeOnly => timeOnly.ToTimeSpan(),
                timeSpan => TimeOnly.FromTimeSpan(timeSpan));

            modelBuilder.Entity<WorkerDto>()
                .Property(e => e.StartWorkTime)
                .HasConversion(timeOnlyConverter)
                .HasColumnType("time(7)");

            base.OnModelCreating(modelBuilder);
        }
    }
}
