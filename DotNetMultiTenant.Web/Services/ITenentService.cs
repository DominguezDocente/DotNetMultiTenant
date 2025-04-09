
using DotNetMultiTenant.Web.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace DotNetMultiTenant.Web.Services
{
    public interface ITenantService
    {
        public string GetTenat();
    }

    public class TenentService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenentService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTenat()
        {
            HttpContext? httpContext = _httpContextAccessor.HttpContext;

            if (httpContext is null)
            {
                return string.Empty;
            }

            AuthenticationTicket? authTicket = DecryptAuthCookie(httpContext);

            if (authTicket is null)
            {
                return string.Empty;
            }

            Claim? claimTenant = authTicket.Principal.Claims.FirstOrDefault(x => x.Type == Constants.CLAIM_TENANT_ID);

            if (claimTenant is null)
            {
                return string.Empty;
            }

            return claimTenant.Value;
        }

        private  static AuthenticationTicket? DecryptAuthCookie(HttpContext httpContext)
        {
            CookieAuthenticationOptions opt = httpContext.RequestServices
                .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
                .Get("Identity.Application");

            string? cookie = opt.CookieManager.GetRequestCookie(httpContext, opt.Cookie.Name!);

            return opt.TicketDataFormat.Unprotect(cookie);
        }
    }
}
