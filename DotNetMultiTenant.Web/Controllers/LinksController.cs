using DotNetMultiTenant.Web.Data;
using DotNetMultiTenant.Web.Data.Entities;
using DotNetMultiTenant.Web.Models;
using DotNetMultiTenant.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DotNetMultiTenant.Web.Controllers
{
    [Authorize]
    public class LinksController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;
        private readonly IChangeTenatService _changeTenatService;
        private readonly ITenantService _tenantService;

        public LinksController(DataContext context, IUserService userService, IChangeTenatService changeTenatService, ITenantService tenentService)
        {
            _context = context;
            _userService = userService;
            _changeTenatService = changeTenatService;
            _tenantService = tenentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = _userService.GetUserId();
            return await ReturnPendingLinks(userId);
        }

        [HttpPost]
        public async Task<IActionResult> Index(Guid companyId, ConnectionStatus connectionStatus)
        {
            string userId = _userService.GetUserId();
            CompanyUserConnection? link = await _context.CompanyUserConnections.FirstOrDefaultAsync(x => x.UserId == userId
                                                                                        && x.CompanyId == companyId
                                                                                        && x.Status == ConnectionStatus.Pending);

            if (link is null)
            {
                ModelState.AddModelError("", "Ha habido un error: vinculación no encontrada");
                return await ReturnPendingLinks(userId);
            }

            if (connectionStatus == ConnectionStatus.Accept)
            {
                CompanyUserPermission nullPermission = new CompanyUserPermission
                {
                    Permission = Permissions.Null,
                    CompanyId = companyId,
                    UserId = userId
                };

                _context.CompanyUserPermissions.Add(nullPermission);
            }

            link.Status = connectionStatus;
            await _context.SaveChangesAsync();
            return RedirectToAction("Change", "Companies");
        }

        [HttpGet]
        public async Task<IActionResult> Link()
        {
            string companyId = _tenantService.GetTenat();

            if (string.IsNullOrEmpty(companyId))
            {
                return RedirectToAction("Index", "Home");
            }

            Guid companyGuidId = new Guid(companyId);

            Company? company = await _context.Companies.FirstOrDefaultAsync(x => x.Id == companyGuidId);
        
            if (company is null)
            {
                return RedirectToAction("Index", "Home");
            }

            LinkUser model = new LinkUser
            {
                CompanyId = company.Id,
                CompanyName = company.Name
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Link(LinkUser model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            IdentityUser? user = await _context.Users.FirstOrDefaultAsync(x => x.Email == model.UserEmail);

            if (user is null)
            {
                ModelState.AddModelError(nameof(model.UserEmail), "No existe usuario con el email indicado");
                return View(model);
            }

            CompanyUserConnection link = new CompanyUserConnection
            {
                CompanyId = model.CompanyId,
                UserId = user.Id,
                Status = ConnectionStatus.Pending,
                CreationDate = DateTime.UtcNow
            };

            _context.CompanyUserConnections.Add(link);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(LinkedUser));
            
        }

        public IActionResult LinkedUser()
        {
            return View();
        }

        private async Task<IActionResult> ReturnPendingLinks(string userId)
        {
            List<CompanyUserConnection> pendingLinks = await _context.CompanyUserConnections.Include(x => x.Company)
                                                                                            .Where(x => x.Status == ConnectionStatus.Pending
                                                                                                        && x.UserId == userId)
                                                                                            .ToListAsync();

            return View(pendingLinks);
        }
    }
}
