using DotNetMultiTenant.Web.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace DotNetMultiTenant.Web.Core.Extensions
{
    public static class TypeExtensions
    {
        public static bool MustOmitTenantValidation(this Type t)
        {
            List<bool> allowToOmitList = new List<bool>()
            {
                t.IsAssignableFrom(typeof(IdentityRole)),
                t.IsAssignableFrom(typeof(IdentityRoleClaim<string>)),
                t.IsAssignableFrom(typeof(IdentityUser)),
                t.IsAssignableFrom(typeof(IdentityUserLogin<string>)),
                t.IsAssignableFrom(typeof(IdentityUserRole<string>)),
                t.IsAssignableFrom(typeof(IdentityUserToken<string>)),
                t.IsAssignableFrom(typeof(IdentityUserClaim<string>)),
                typeof(ICommonEntity).IsAssignableFrom(t)
            };

            bool result = allowToOmitList.Aggregate((b1, b2) => b1 || b2);

            return result;
        }
    }
}
