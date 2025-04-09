using System.Diagnostics;
using DotNetMultiTenant.Web.Data;
using DotNetMultiTenant.Web.Data.Entities;
using DotNetMultiTenant.Web.Models;
using DotNetMultiTenant.Web.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetMultiTenant.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await BuildHomeIndexViewModel());
        }


        [HttpPost]
        [HasPermission(Permissions.Products_Create)]
        public async Task<IActionResult> Product(Product product)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            HomeIndexViewModel model = await BuildHomeIndexViewModel();
            return View(nameof(Index), model);
        }

        private async Task<HomeIndexViewModel> BuildHomeIndexViewModel()
        {
            IEnumerable<Product> products = await _context.Products.ToListAsync(); 
            IEnumerable<Country> countries = await _context.Countries.ToListAsync();

            HomeIndexViewModel model = new HomeIndexViewModel
            {
                Products = products,
                Countries = countries
            };

            return model;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
