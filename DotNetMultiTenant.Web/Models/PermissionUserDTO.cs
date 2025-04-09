using DotNetMultiTenant.Web.Data.Entities;

namespace DotNetMultiTenant.Web.Models
{
    public class PermissionUserDTO
    {
        public Permissions Permission { get; set; }
        public bool HasPermission { get; set; }
        public string? Description { get; set; }
    }
}