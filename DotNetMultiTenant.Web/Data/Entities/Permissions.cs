using DotNetMultiTenant.Web.Core.Attributes;
using System.ComponentModel.DataAnnotations;

namespace DotNetMultiTenant.Web.Data.Entities
{
    public enum Permissions
    {
        [Hide]
        Null = 0, // Permiso por defecto

        [Display(Description = "Puede crear productos")]
        Products_Create = 1,

        [Display(Description = "Puede leer productos")]
        Products_Read = 2,

        Users_Link = 3,

        Permissions_Read = 4,

        Permissions_Modify = 5,
    }
}
