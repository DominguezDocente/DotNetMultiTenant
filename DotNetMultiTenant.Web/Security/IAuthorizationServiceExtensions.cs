using DotNetMultiTenant.Web.Core;
using DotNetMultiTenant.Web.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace DotNetMultiTenant.Web.Security
{
    public static class IAuthorizationServiceExtensions
    {
        public static async Task<bool> HasPermission(this IAuthorizationService authorizationService, ClaimsPrincipal user, Permissions permission)
        {
            if (!user.Identity!.IsAuthenticated)
            {
                return false;
            }

            string policyName = $"{Constants.POLICY_PREFIX}{permission}";
            var result = await authorizationService.AuthorizeAsync(user, policyName);
            return result.Succeeded;
        }
    }
}
