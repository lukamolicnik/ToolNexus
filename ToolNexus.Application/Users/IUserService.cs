using ToolNexus.Application.Users.DTOs;
using ToolNexus.Domain.Users;

namespace ToolNexus.Application.Users
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, string createdBy);
        Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto, string updatedBy);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ToggleUserStatusAsync(int id);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);

        Task<AuthenticationResult> AuthenticateAsync(LoginDto loginDto);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        Task<UserRoleDto> GetUserRoleByIdAsync(int roleId);
        Task<List<UserRoleDto>> GetAllUserRolesAsync();
    }
}
