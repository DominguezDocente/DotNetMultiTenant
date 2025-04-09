namespace DotNetMultiTenant.Web.Models
{
    public class ManagePermissionsDTO
    {
        public string UserId { get; set; } = null!;
        public string? Email { get; set; }
        public List<PermissionUserDTO> Permissions { get; set; } = [];
        
    }
}
