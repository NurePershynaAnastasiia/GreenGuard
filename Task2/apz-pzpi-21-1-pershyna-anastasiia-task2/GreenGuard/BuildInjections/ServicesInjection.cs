using GreenGuard.Services;

namespace GreenGuard.BuildInjections
{
    internal static class ServicesInjection
    {
        internal static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<WateringService>();
            services.AddScoped<SalaryService>();
            services.AddScoped<TaskService>();
        }
    }
}
