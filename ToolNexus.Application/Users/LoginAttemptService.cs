using Microsoft.Extensions.Caching.Memory;
using ToolNexus.Application.Users.DTOs;

namespace ToolNexus.Application.Users
{
    public class LoginAttemptService : ILoginAttemptService
    {
        private readonly IMemoryCache _cache;
        private const int MaxAttemptsPerUsername = 5;
        private const int MaxAttemptsPerIp = 10;
        private const int LockoutDurationMinutes = 15;
        private const int TimeWindowMinutes = 15;

        public LoginAttemptService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<bool> IsRateLimitedAsync(string username, string ipAddress)
        {
            var userAttemptsKey = $"login_attempts_user_{username}";
            var ipAttemptsKey = $"login_attempts_ip_{ipAddress}";
            var userLockoutKey = $"lockout_user_{username}";
            var ipLockoutKey = $"lockout_ip_{ipAddress}";

            // Preveri če je uporabnik ali IP v lockout
            if (_cache.TryGetValue<bool>(userLockoutKey, out _) || 
                _cache.TryGetValue<bool>(ipLockoutKey, out _))
            {
                return true;
            }

            // Pridobi število poskusov
            var userAttempts = await GetFailedAttemptsCountAsync(username, ipAddress, TimeSpan.FromMinutes(TimeWindowMinutes));
            
            // Če preseženo število poskusov, nastavi lockout
            if (userAttempts >= MaxAttemptsPerUsername)
            {
                _cache.Set(userLockoutKey, true, TimeSpan.FromMinutes(LockoutDurationMinutes));
                return true;
            }

            var ipAttempts = _cache.Get<List<LoginAttemptDto>>(ipAttemptsKey) ?? new List<LoginAttemptDto>();
            var recentIpAttempts = ipAttempts.Count(a => !a.Success && 
                a.AttemptTime > DateTime.Now.AddMinutes(-TimeWindowMinutes));
            
            if (recentIpAttempts >= MaxAttemptsPerIp)
            {
                _cache.Set(ipLockoutKey, true, TimeSpan.FromMinutes(LockoutDurationMinutes));
                return true;
            }

            return false;
        }

        public async Task RecordLoginAttemptAsync(string username, string ipAddress, bool success)
        {
            var userAttemptsKey = $"login_attempts_user_{username}";
            var ipAttemptsKey = $"login_attempts_ip_{ipAddress}";

            var attempt = new LoginAttemptDto
            {
                Username = username,
                IpAddress = ipAddress,
                AttemptTime = DateTime.Now,
                Success = success
            };

            // Shrani poskus za uporabnika
            var userAttempts = _cache.Get<List<LoginAttemptDto>>(userAttemptsKey) ?? new List<LoginAttemptDto>();
            userAttempts.Add(attempt);
            
            // Ohrani samo poskuse iz zadnjih 24 ur
            userAttempts = userAttempts.Where(a => a.AttemptTime > DateTime.Now.AddHours(-24)).ToList();
            _cache.Set(userAttemptsKey, userAttempts, TimeSpan.FromHours(24));

            // Shrani poskus za IP
            var ipAttempts = _cache.Get<List<LoginAttemptDto>>(ipAttemptsKey) ?? new List<LoginAttemptDto>();
            ipAttempts.Add(attempt);
            
            // Ohrani samo poskuse iz zadnjih 24 ur
            ipAttempts = ipAttempts.Where(a => a.AttemptTime > DateTime.Now.AddHours(-24)).ToList();
            _cache.Set(ipAttemptsKey, ipAttempts, TimeSpan.FromHours(24));

            // Če je bila prijava uspešna, počisti neuspešne poskuse
            if (success)
            {
                await ClearFailedAttemptsAsync(username, ipAddress);
            }

            await Task.CompletedTask;
        }

        public async Task<int> GetFailedAttemptsCountAsync(string username, string ipAddress, TimeSpan timeWindow)
        {
            var userAttemptsKey = $"login_attempts_user_{username}";
            var attempts = _cache.Get<List<LoginAttemptDto>>(userAttemptsKey) ?? new List<LoginAttemptDto>();
            
            var cutoffTime = DateTime.Now.Subtract(timeWindow);
            var failedAttempts = attempts.Count(a => !a.Success && a.AttemptTime > cutoffTime);
            
            return await Task.FromResult(failedAttempts);
        }

        public async Task ClearFailedAttemptsAsync(string username, string ipAddress)
        {
            var userLockoutKey = $"lockout_user_{username}";
            var ipLockoutKey = $"lockout_ip_{ipAddress}";
            
            _cache.Remove(userLockoutKey);
            _cache.Remove(ipLockoutKey);
            
            await Task.CompletedTask;
        }
    }
}