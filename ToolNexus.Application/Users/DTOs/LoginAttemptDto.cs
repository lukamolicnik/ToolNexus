namespace ToolNexus.Application.Users.DTOs
{
    public class LoginAttemptDto
    {
        public string Username { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime AttemptTime { get; set; }
        public bool Success { get; set; }
    }
}