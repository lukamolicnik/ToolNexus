using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolNexus.Application.Tools.DTOs
{
    public class CreateToolDto
    {
        [Required(ErrorMessage = "Koda orodja je obvezna")]
        [StringLength(50, MinimumLength = 3)]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ime je obvezno")]
        [StringLength(50, ErrorMessage = "Ime ne sme biti daljše od 50 znakov")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Opis ne sme biti daljši od 100 znakov")]
        public string? Description { get; set; }

        public int? ToolCategoryId { get; set; }
    }
}
