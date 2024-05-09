using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GreenGuard.BuildInjections
{
    public class ErrorResponseInjection
    {
        private readonly RequestDelegate _next;

        public ErrorResponseInjection(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == 401)
            {
                context.Response.Clear(); // Очищаємо відповідь перед відправленням нового статусу та тіла
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("401: Користувач не авторизований");
            }
            else if (context.Response.StatusCode == 403)
            {
                await context.Response.WriteAsync("403: Доступ заборонено");
            }
            else
            {
                await context.Response.WriteAsync($"Error: response status is {context.Response.StatusCode}");
            }
        }
    }

    public static class ErrorResponseInjectionExtensions
    {
        public static IApplicationBuilder UseErrorResponseInjection(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorResponseInjection>();
        }
    }

}
