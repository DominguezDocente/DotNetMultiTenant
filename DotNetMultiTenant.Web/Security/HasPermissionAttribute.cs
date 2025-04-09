using DotNetMultiTenant.Web.Core;
using DotNetMultiTenant.Web.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace DotNetMultiTenant.Web.Security
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public Permissions Permissions
        {
            get
            {
                // Policy = HasPermissionProduct_Create
                if (Enum.TryParse(typeof(Permissions), Policy!.Substring(Constants.POLICY_PREFIX.Length), ignoreCase: true, out object? permission))
                {
                    return (Permissions)permission!;
                }

                return Permissions.Null;
            }

            set
            {
                Policy = $"{Constants.POLICY_PREFIX}{value.ToString()}";
            }
        }

        public HasPermissionAttribute(Permissions permissions)
        {
            Permissions = permissions;
        }
    }
}
