using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotNetMultiTenant.Web.Data.Entities
{
    public class CompanyUserPermission : ICommonEntity
    {
        public Guid CompanyId { get; set; }
        public string UserId { get; set; } = null!;
        public Company Company { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;
        public Permissions Permission { get; set; }
    }
}
