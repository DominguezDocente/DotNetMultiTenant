using System.ComponentModel.DataAnnotations;

namespace DotNetMultiTenant.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo {0} debe ser un correo válido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Recuérdame")]
        public bool Rememberme { get; set; }
    }
}
