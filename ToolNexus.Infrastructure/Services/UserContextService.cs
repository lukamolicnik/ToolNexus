using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;

namespace ToolNexus.Infrastructure.Services
{
    public interface IUserContextService
    {
        string GetCurrentUserId();
        string GetCurrentUserName();
        void SetUserContext(string userId, string userName);
    }

    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        // AsyncLocal za shranjevanje uporabniškega konteksta preko async klicev
        private static readonly AsyncLocal<UserContext> _userContext = new AsyncLocal<UserContext>();

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            InitializeUserContext();
        }

        private void InitializeUserContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userId = httpContext.User.FindFirst("UserId")?.Value ?? "0";
                var userName = httpContext.User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
                
                _userContext.Value = new UserContext
                {
                    UserId = userId,
                    UserName = userName
                };
            }
        }

        public void SetUserContext(string userId, string userName)
        {
            _userContext.Value = new UserContext
            {
                UserId = userId,
                UserName = userName
            };
        }

        public string GetCurrentUserId()
        {
            // Najprej poskusi uporabiti AsyncLocal kontekst
            if (_userContext.Value != null && !string.IsNullOrEmpty(_userContext.Value.UserId))
                return _userContext.Value.UserId;

            // Če ni v AsyncLocal, poskusi pridobiti iz HttpContext
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = httpContext.User.FindFirst("UserId")?.Value;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    // Shrani v AsyncLocal za prihodnje klice
                    if (_userContext.Value == null)
                        _userContext.Value = new UserContext();
                    _userContext.Value.UserId = userIdClaim;
                    return userIdClaim;
                }
            }
            
            return "0";
        }

        public string GetCurrentUserName()
        {
            // Najprej poskusi uporabiti AsyncLocal kontekst
            if (_userContext.Value != null && !string.IsNullOrEmpty(_userContext.Value.UserName))
                return _userContext.Value.UserName;

            // Če ni v AsyncLocal, poskusi pridobiti iz HttpContext
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var userNameClaim = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                if (!string.IsNullOrEmpty(userNameClaim))
                {
                    // Shrani v AsyncLocal za prihodnje klice
                    if (_userContext.Value == null)
                        _userContext.Value = new UserContext();
                    _userContext.Value.UserName = userNameClaim;
                    return userNameClaim;
                }
                
                // Poskusi tudi Identity.Name
                var identityName = httpContext.User.Identity.Name;
                if (!string.IsNullOrEmpty(identityName))
                {
                    if (_userContext.Value == null)
                        _userContext.Value = new UserContext();
                    _userContext.Value.UserName = identityName;
                    return identityName;
                }
            }
            
            return "System";
        }

        private class UserContext
        {
            public string UserId { get; set; } = "0";
            public string UserName { get; set; } = "System";
        }
    }
}