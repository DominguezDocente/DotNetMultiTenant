using System.ComponentModel.DataAnnotations;

namespace DotNetMultiTenant.Web.Models
{
    public class CreateCompany
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Name { get; set; }
    }
}
