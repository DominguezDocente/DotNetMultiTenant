using System.Security.Claims;

namespace DotNetMultiTenant.Web.Services
{
    public interface IUserService
    {
        public string GetUserId();
    }

    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContext;
        public UserService(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }

        public string GetUserId()
        {
            if (_httpContext.HttpContext.User.Identity!.IsAuthenticated)
            {
                Claim? claimId = _httpContext.HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault();

                if (claimId is null)
                {
                    throw new ApplicationException("El usuario no tiene el claim del ID");
                }

                return claimId.Value;
            }

            else
            {
                throw new ApplicationException("El usuario no está autenticado");
            }
        }
    }
}
