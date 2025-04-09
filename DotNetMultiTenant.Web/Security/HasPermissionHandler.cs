using DotNetMultiTenant.Web.Data;
using DotNetMultiTenant.Web.Data.Entities;
using DotNetMultiTenant.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DotNetMultiTenant.Web.Security
{
    public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
    {
        private readonly ITenantService _tenantService;
        private readonly IUserService _userService;
        private readonly DataContext _context;

        public HasPermissionHandler(ITenantService tenantService, IUserService userService, DataContext context)
        {
            _tenantService = tenantService;
            _userService = userService;
            _context = context;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
        {
            Permissions permission = requirement.Permission;
            string userId = _userService.GetUserId();
            Guid tenantId = new Guid(_tenantService.GetTenat());

            bool hasPermission = await _context.CompanyUserPermissions.AnyAsync(x => x.UserId == userId 
                                                                                     && x.CompanyId == tenantId
                                                                                     && x.Permission == permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}
