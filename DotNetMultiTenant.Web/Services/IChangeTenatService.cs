using DotNetMultiTenant.Web.Core;
using DotNetMultiTenant.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DotNetMultiTenant.Web.Services
{
    public interface IChangeTenatService
    {
        public Task ChangeTenant(Guid companyId, string userId);
    }

    public class ChangeTenatService : IChangeTenatService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly DataContext _context;

        public ChangeTenatService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, DataContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task ChangeTenant(Guid companyId, string userId)
        {
            IdentityUser user = await _userManager.FindByIdAsync(userId);

            IdentityUserClaim<string>? claimExistingTenant = await _context.UserClaims
                .FirstOrDefaultAsync(x => x.ClaimType == Constants.CLAIM_TENANT_ID 
                                            && x.UserId == userId);

            if (claimExistingTenant is not null)
            {
                _context.Remove(claimExistingTenant);
            }

            Claim newTenantClaim = new Claim(Constants.CLAIM_TENANT_ID, companyId.ToString());

            await _userManager.AddClaimAsync(user, newTenantClaim);

            await _signInManager.SignInAsync(user, isPersistent: true);
        }
    }
}
