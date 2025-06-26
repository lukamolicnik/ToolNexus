using ToolNexus.Application.Users.DTOs;

namespace ToolNexus.Application.Users
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateAsync(LoginDto loginDto);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
