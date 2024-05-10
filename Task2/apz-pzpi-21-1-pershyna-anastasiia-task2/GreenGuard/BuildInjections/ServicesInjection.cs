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
            services.AddScoped<IPasswordHasher<WorkerDto>, PasswordHasher<WorkerDto>>();
        }
    }
}
