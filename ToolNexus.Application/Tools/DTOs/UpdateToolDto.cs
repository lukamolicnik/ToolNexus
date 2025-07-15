using System.ComponentModel.DataAnnotations;

namespace ToolNexus.Application.Tools.DTOs
{
    public class UpdateToolDto
    {
        [Required(ErrorMessage = "Ime je obvezno")]
        [StringLength(50, ErrorMessage = "Ime ne sme biti daljše od 50 znakov")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Opis ne sme biti daljši od 100 znakov")]
        public string? Description { get; set; }
    }
}