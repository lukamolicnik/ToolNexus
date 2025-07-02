namespace ToolNexus.Domain.Users
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required int UserRoleId { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public virtual UserRole UserRole { get; set; } = null!;

        // Computed property za prikaz polnega imena
        public string FullName => $"{FirstName} {LastName}";
    }
}
