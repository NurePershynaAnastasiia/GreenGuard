using GreenGuard.Dto;
using GreenGuard.Services;
using Microsoft.AspNetCore.Identity;

namespace GreenGuard.BuildInjections
{
    internal static class ServicesInjection
    {
        internal static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<WateringService>();
            services.AddScoped<SalaryService>();
            services.AddScoped<TaskService>();
            services.AddScoped<WorkerService>();
            services.AddScoped<PlantService>();
            services.AddScoped<FertilizerService>();
            services.AddScoped<PestService>();
            services.AddScoped<PlantTypeService>();
            services.AddScoped<WorkingScheduleService>();
            services.AddScoped<BackupService>();
            services.AddScoped<PlantStatusService>();
            services.AddScoped<IPasswordHasher<WorkerDto>, PasswordHasher<WorkerDto>>();
        }
    }
}
