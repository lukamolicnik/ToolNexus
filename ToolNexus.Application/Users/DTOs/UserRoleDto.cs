using ToolNexus.Domain.Users;

namespace ToolNexus.Application.Users.DTOs
{
    public class UserRoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        public static UserRoleDto FromUserRole(UserRole userRole)
        {
            return new UserRoleDto
            {
                Id = userRole.Id,
                Name = userRole.Name,
                Description = userRole.Description ?? string.Empty,
            };
        }
    }
}
