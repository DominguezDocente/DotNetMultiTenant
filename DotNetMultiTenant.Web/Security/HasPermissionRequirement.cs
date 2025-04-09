using DotNetMultiTenant.Web.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace DotNetMultiTenant.Web.Security
{
    public class HasPermissionRequirement : IAuthorizationRequirement
    {
        public HasPermissionRequirement(Permissions permission)
        {
            Permission = permission;
        }

        public Permissions Permission { get; }
    }
}
