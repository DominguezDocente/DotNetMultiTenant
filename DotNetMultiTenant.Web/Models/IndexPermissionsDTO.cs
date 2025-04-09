namespace DotNetMultiTenant.Web.Models
{
    public class IndexPermissionsDTO
    {
        public string CompanyName { get; set; } = null!;
        public IEnumerable<UserDTO> Employees { get; set; } = null!;
    }
}
