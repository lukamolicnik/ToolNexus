using System.Security.Claims;
using ToolNexus.Infrastructure.Services;

namespace ToolNexus.WebUI.Server.Middleware
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserContextService userContextService)
        {
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst("UserId")?.Value ?? "0";
                var userName = context.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
                
                // Nastavi uporabniški kontekst za to zahtevo
                userContextService.SetUserContext(userId, userName);
            }

            await _next(context);
        }
    }

    public static class UserContextMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserContext(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserContextMiddleware>();
        }
    }
}