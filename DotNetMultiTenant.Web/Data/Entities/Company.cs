using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DotNetMultiTenant.Web.Data.Entities
{
    public class Company : ICommonEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string CreatorUserId { get; set; } = null!;
        public IdentityUser? CreatorUser { get; set; }
        public List<CompanyUserPermission>? CompanyUserPermissions { get; set; }
    }
}
