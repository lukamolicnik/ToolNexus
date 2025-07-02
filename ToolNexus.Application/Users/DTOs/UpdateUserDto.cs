using System.ComponentModel.DataAnnotations;
using ToolNexus.Domain.Users;

namespace ToolNexus.Application.Users.DTOs
{
    public class UpdateUserDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ime je obvezno")]
        [StringLength(100, ErrorMessage = "Ime ne sme biti daljše od 100 znakov")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priimek je obvezen")]
        [StringLength(100, ErrorMessage = "Priimek ne sme biti daljši od 100 znakov")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obvezen")]
        [EmailAddress(ErrorMessage = "Neveljaven email format")]
        [StringLength(200, ErrorMessage = "Email ne sme biti daljši od 200 znakov")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vloga je obvezna")]
        public int UserRoleId { get; set; }

        public bool IsActive { get; set; }

        // Optional password change
        [StringLength(100, ErrorMessage = "Geslo mora biti dolgo med 6 in 100 znakov", MinimumLength = 6)]
        public string? NewPassword { get; set; }
    }
}