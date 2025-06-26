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
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
    }
}
