using DotNetMultiTenant.Web.Core;
using DotNetMultiTenant.Web.Data;
using DotNetMultiTenant.Web.Data.Entities;
using DotNetMultiTenant.Web.Models;
using DotNetMultiTenant.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DotNetMultiTenant.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly DataContext _context;
        private readonly IChangeTenatService _changeTenatService;

        public UsersController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, DataContext context, IChangeTenatService changeTenatService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _changeTenatService = changeTenatService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            IdentityUser user = new IdentityUser() { Email = model.Email, UserName = model.Email };

            IdentityResult result = await _userManager.CreateAsync(user, password: model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(model.Email,
                   model.Password, model.Rememberme, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                IdentityUser user = await _userManager.FindByEmailAsync(model.Email);

                List<Guid> linkedCompaniesIds = await _context.CompanyUserPermissions.Include(x => x.Company)
                                                                                    .Where(x => x.UserId == user.Id && x.Permission == Permissions.Null)
                                                                                    .OrderBy(x => x.CompanyId)
                                                                                    .Take(2)
                                                                                    .Select(x => x.CompanyId!)
                                                                                    .ToListAsync();

                if (linkedCompaniesIds.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (linkedCompaniesIds.Count == 1)
                {
                    await _changeTenatService.ChangeTenant(linkedCompaniesIds[0], user.Id);
                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Change", "Companies");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrecto.");
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
