using System.ComponentModel.DataAnnotations;

namespace ToolNexus.Application.Users.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Uporabniško ime je obvezno")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Geslo je obvezno")]
        public string Password { get; set; } = string.Empty;
    }
}
