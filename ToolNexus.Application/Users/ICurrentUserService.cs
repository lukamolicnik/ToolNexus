using ToolNexus.Application.Users.DTOs;
using ToolNexus.Domain.Users;

namespace ToolNexus.Application.Users
{
    public interface ICurrentUserService
    {
        UserDto? CurrentUser { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(UserRole role);
        bool IsWorker { get; }
        bool IsProductionSupervisor { get; }
        bool IsAdministrator { get; }
        Task SetCurrentUserAsync(UserDto user);
        Task ClearCurrentUserAsync();
    }
}