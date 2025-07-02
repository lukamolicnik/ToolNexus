namespace ToolNexus.Domain.Users
{
    public class UserRole
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
