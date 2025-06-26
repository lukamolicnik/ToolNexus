namespace ToolNexus.Application.Users.DTOs
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserDto? User { get; set; }

        public static AuthenticationResult SuccessResult(UserDto user)
        {
            return new AuthenticationResult
            {
                Success = true,
                Message = "Prijava uspešna",
                User = user
            };
        }

        public static AuthenticationResult FailureResult(string message)
        {
            return new AuthenticationResult
            {
                Success = false,
                Message = message,
                User = null
            };
        }
    }
}
