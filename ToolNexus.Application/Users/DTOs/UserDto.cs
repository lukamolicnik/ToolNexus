using ToolNexus.Domain.Users;

namespace ToolNexus.Application.Users.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public bool IsActive { get; set; }
        public string FullName { get; set; } = string.Empty;

        public static UserDto FromUser(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserRole = user.UserRole,
                IsActive = user.IsActive,
                FullName = user.FullName
            };
        }
    }
}
