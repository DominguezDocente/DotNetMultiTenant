using DotNetMultiTenant.Web.Core.Attributes;
using DotNetMultiTenant.Web.Data;
using DotNetMultiTenant.Web.Data.Entities;
using DotNetMultiTenant.Web.Models;
using DotNetMultiTenant.Web.Security;
using DotNetMultiTenant.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DotNetMultiTenant.Web.Controllers
{
    [Authorize]
    public class PermissionsController : Controller
    {
        private readonly DataContext _context;
        private readonly ITenantService _tenantService;

        public PermissionsController(DataContext context, ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        [HasPermission(Permissions.Permissions_Read)]
        public async Task<IActionResult> Index()
        {
            Guid tenantId = new Guid(_tenantService.GetTenat());
            IndexPermissionsDTO? model = await _context.Companies.Include(x => x.CompanyUserPermissions)
                                                                    .ThenInclude(y => y.User)
                                                                .Where(x => x.Id == tenantId)
                                                                .Select(x => new IndexPermissionsDTO
                                                                {
                                                                    CompanyName = x.Name,
                                                                    Employees = x.CompanyUserPermissions.Select(z => new UserDTO
                                                                    {
                                                                        Email = z.User!.Email
                                                                    }).Distinct()
                                                                }).FirstOrDefaultAsync();

            return View(model);
        }

        [HasPermission(Permissions.Permissions_Read)]
        public async Task<IActionResult> Manage(string email)
        {
            Guid tenantId = new Guid(_tenantService.GetTenat());
            string? userId = await _context.Users.Where(x => x.Email == email)
                                                 .Select(x => x.Id)
                                                 .FirstOrDefaultAsync();
        
            if (userId is null)
            {
                return RedirectToAction("Index", "Permissions");
            }

            List<CompanyUserPermission> permissions = await _context.CompanyUserPermissions.Where(x => x.CompanyId == tenantId
                                                                                                       && x.UserId == userId
                                                                                                       && x.Permission != Permissions.Null)
                                                                                           .ToListAsync();

            Dictionary<Permissions, CompanyUserPermission> userPermissionDictionary = permissions.ToDictionary(x => x.Permission);

            ManagePermissionsDTO model = new ManagePermissionsDTO();
            model.UserId = userId;
            model.Email = email;

            foreach(Permissions permission in Enum.GetValues<Permissions>())
            {
                FieldInfo? field = typeof(Permissions).GetField(permission.ToString());
                bool hide = field.IsDefined(typeof(HideAttribute), false);

                if (hide)
                {
                    continue;
                }

                string description = permission.ToString();

                if (field.IsDefined(typeof(DisplayAttribute), false))
                {
                    DisplayAttribute displayAttribute = (DisplayAttribute)Attribute.GetCustomAttribute(field, typeof(DisplayAttribute))!;

                    description = displayAttribute.Description!;
                }

                model.Permissions.Add(new PermissionUserDTO
                {
                    Description = description,
                    Permission = permission,
                    HasPermission = userPermissionDictionary.ContainsKey(permission)
                });
            }

            return View(model);
        }

        [HttpPost]
        [HasPermission(Permissions.Permissions_Modify)]
        public async Task<IActionResult> Manage(ManagePermissionsDTO dto)
        {
            Guid tenantId = new Guid(_tenantService.GetTenat());

            // Siempre se agrega permiso por defecto
            dto.Permissions.Add(new PermissionUserDTO
            {
                HasPermission = true,
                Permission = Permissions.Null
            });

            // Eliminacion de todos los permisos
            await _context.Database
                .ExecuteSqlInterpolatedAsync($@"DELETE 
                                                    CompanyUserPermissions
                                                WHERE
                                                    UserId = {dto.UserId}
                                                AND
                                                    CompanyId = {tenantId}");

            // Filtrado de permisos a agregar
            IEnumerable<CompanyUserPermission> filteredPermissions = dto.Permissions.Where(x => x.HasPermission)
                                                                                    .Select(x => new CompanyUserPermission 
                                                                                    {
                                                                                        CompanyId = tenantId,
                                                                                        UserId = dto.UserId,
                                                                                        Permission = x.Permission,
                                                                                    });

            // Agregar permisos a tabla
            _context.AddRange(filteredPermissions);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
