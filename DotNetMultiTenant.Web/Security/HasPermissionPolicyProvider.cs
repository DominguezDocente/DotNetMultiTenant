using DotNetMultiTenant.Web.Core;
using DotNetMultiTenant.Web.Data.Entities;
using Microsoft.AspNetCore.Authorization;

namespace DotNetMultiTenant.Web.Security
{
    public class HasPermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return Task.FromResult(new AuthorizationPolicyBuilder("Identity.Application").RequireAuthenticatedUser().Build());
        }

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return Task.FromResult<AuthorizationPolicy?>(null!);
        }

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(Constants.POLICY_PREFIX, StringComparison.OrdinalIgnoreCase) &&
                Enum.TryParse(typeof(Permissions), policyName.Substring(Constants.POLICY_PREFIX.Length),
                               out object? objPermission))
            {
                Permissions permission = (Permissions)objPermission!;
                AuthorizationPolicyBuilder policy = new AuthorizationPolicyBuilder("Identity.Application");
                policy.AddRequirements(new HasPermissionRequirement(permission));
                return Task.FromResult<AuthorizationPolicy?>(policy.Build());
            }

            return Task.FromResult<AuthorizationPolicy?>(null!);
        }
    }
}
