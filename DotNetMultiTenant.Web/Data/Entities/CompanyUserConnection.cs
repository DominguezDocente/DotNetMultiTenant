using Microsoft.AspNetCore.Identity;

namespace DotNetMultiTenant.Web.Data.Entities
{
    public class CompanyUserConnection : ICommonEntity
    {
        public int Id { get; set; }
        public Guid CompanyId { get; set; }
        public string UserId { get; set; } = null!;
        public ConnectionStatus Status { get; set; }
        public DateTime CreationDate { get; set; }
        public Company Company { get; set; }
        public IdentityUser User { get; set; }

    }
}
