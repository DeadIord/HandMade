using HandMade.Data;
using HandMade.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static HandMade.Controllers.ProductController;

namespace HandMade.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext db;
      
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            db = context;
        }

        public async Task<IActionResult> Main()
        {
            return View();
        }

        public async Task<IActionResult> IndexAsync(string searchString, string filter)
        {


            var product = db.Products.Include(p => p.Categories).AsQueryable();

            var activeCategories = db.Categories
                .Where(c => c.ActiveCategories == "Активная")
                .Select(c => c.CategoriesId)
                .ToList();

            var a = db.Categories
                .Where(c => activeCategories.Contains(c.CategoriesId))
                .Select(p => new SelectOptions
                {
                    value = p.CategoriesId,
                    text = p.NameOfTheCategories
                })
                .ToList();

            TempData["Categories"] = a;

            if (!string.IsNullOrEmpty(searchString))
            {
                product = product.Where(v => v.NameOfTheProduct.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(filter))
            {
                product = product.Where(v => v.Categories.NameOfTheCategories == filter);
            }

            product = product.Where(p => activeCategories.Contains(p.Categories.CategoriesId));

            return View(await product.ToListAsync());
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
