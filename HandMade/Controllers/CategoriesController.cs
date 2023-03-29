using HandMade.Data;
using HandMade.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HandMade.Controllers
{
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db;
        public CategoriesController(ApplicationDbContext context)
        {
            db = context;
        }
        public async Task<IActionResult> ListCategories()
        {
       
            return View(await db.Categories.ToListAsync());
        }


        public IActionResult AddCategories()
        {
          

            return View();

        }
   

        public async Task<IActionResult> EditCategories(int? id)
        {


            if (id != null)
            {
                Categories computers = await db.Categories.FirstOrDefaultAsync(p => p.CategoriesId == id);
              

                if (computers != null)
                    return View(computers);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> AddCategories(Categories computers)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(computers);
                await db.SaveChangesAsync();
                return RedirectToAction("ListCategories");
            }
            else
            {

                return View(computers);
            }

        }
        [HttpPost]
        public async Task<IActionResult> EditCategories(Categories computers)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Update(computers);
                await db.SaveChangesAsync();
                return RedirectToAction("ListCategories");
            }
            else
            {
          
                return View(computers);
            }

        }



        [HttpGet]
        [ActionName("DeleteComputer")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Categories computers = await db.Categories.FirstOrDefaultAsync(p => p.CategoriesId == id);
                if (computers != null)
                    return View(computers);
            }
            return NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                var cater = await db.Categories
               .Include(p => p.Products)
               .SingleAsync(p => p.CategoriesId == id);

                var z = await db.Products
               .Include(p => p.Carts)
               .SingleAsync(p => p.ProductsId == id);

                db.Carts.RemoveRange(z.Carts);
                db.Products.RemoveRange(cater.Products);
                db.Categories.Remove(cater);

                await db.SaveChangesAsync();
                return RedirectToAction("ListCategories");
            }
            return NotFound();
        }
    }
}
