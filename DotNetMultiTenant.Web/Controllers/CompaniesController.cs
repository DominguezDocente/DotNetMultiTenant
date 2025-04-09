using DotNetMultiTenant.Web.Data;
using DotNetMultiTenant.Web.Data.Entities;
using DotNetMultiTenant.Web.Models;
using DotNetMultiTenant.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DotNetMultiTenant.Web.Controllers
{
    [Authorize]
    public class CompaniesController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;
        private readonly IChangeTenatService _changeTenatService;

        public CompaniesController(DataContext context, IUserService userService, IChangeTenatService changeTenatService)
        {
            _context = context;
            _userService = userService;
            _changeTenatService = changeTenatService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCompany model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Company company = new Company
            {
                Name = model.Name
            };

            string userId = _userService.GetUserId();
            company.CreatorUserId = userId;
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Asignación de todos los permisos a usuario creador
            List<CompanyUserPermission> companyUserPermissions = new List<CompanyUserPermission>();

            foreach(Permissions permission in Enum.GetValues<Permissions>())
            {
                companyUserPermissions.Add(new CompanyUserPermission
                {
                    CompanyId = company.Id,
                    UserId = userId,
                    Permission = permission
                });
            }

            _context.AddRange(companyUserPermissions);
            await _context.SaveChangesAsync();

            await _changeTenatService.ChangeTenant(company.Id, userId);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Change()
        {
            string userId = _userService.GetUserId();
            List<Company> companies = await _context.CompanyUserPermissions.Include(x => x.Company)
                                                                           .Where(x => x.UserId == userId)
                                                                           .Select(x => x.Company!)
                                                                           .Distinct()
                                                                           .ToListAsync();

            return View(companies);
        }

        [HttpPost]
        public async Task<IActionResult> Change(Guid id)
        {
            string userId = _userService.GetUserId();
            await _changeTenatService.ChangeTenant(id, userId);
            return RedirectToAction("Index", "Home");
        }
    }
}
