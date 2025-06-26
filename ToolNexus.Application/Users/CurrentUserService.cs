using Microsoft.Extensions.Caching.Memory;
using ToolNexus.Application.Users;
using ToolNexus.Application.Users.DTOs;
using ToolNexus.Domain.Users;

namespace ToolNexus.WebUI.Server.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IMemoryCache _cache;
        private static readonly object _lock = new object();

        // Za Blazor Server uporabljamo statičen slovar, ker je vsak circuit povezan z eno instanco
        private static UserDto? _currentUser;

        public CurrentUserService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public UserDto? CurrentUser
        {
            get
            {
                lock (_lock)
                {
                    return _currentUser;
                }
            }
        }

        public bool IsAuthenticated => CurrentUser != null;

        public bool IsInRole(UserRole role) => CurrentUser?.Role == role;

        public bool IsWorker => IsInRole(UserRole.Worker);

        public bool IsProductionSupervisor => IsInRole(UserRole.ProductionSupervisor);

        public bool IsAdministrator => IsInRole(UserRole.Administrator);

        public Task SetCurrentUserAsync(UserDto user)
        {
            Console.WriteLine($"Setting current user: {user.FullName} (Role: {user.Role})");

            lock (_lock)
            {
                _currentUser = user;
            }

            Console.WriteLine("User saved to static variable");
            return Task.CompletedTask;
        }

        public Task ClearCurrentUserAsync()
        {
            Console.WriteLine("Clearing current user");

            lock (_lock)
            {
                _currentUser = null;
            }

            Console.WriteLine("User cleared from static variable");
            return Task.CompletedTask;
        }
    }
}