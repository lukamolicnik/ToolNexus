using ToolNexus.Application.Users.DTOs;

namespace ToolNexus.Application.Users
{
    public interface ILoginAttemptService
    {
        Task<bool> IsRateLimitedAsync(string username, string ipAddress);
        Task RecordLoginAttemptAsync(string username, string ipAddress, bool success);
        Task ClearFailedAttemptsAsync(string username, string ipAddress);
    }
}