namespace DotNetMultiTenant.Web.Data.Entities
{
    public class Product : ITenantEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string TenantId { get; set; } = null!;
    }
}
