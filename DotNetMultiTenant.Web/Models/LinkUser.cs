namespace DotNetMultiTenant.Web.Models
{
    public class LinkUser
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string UserEmail { get; set; }
    }
}
