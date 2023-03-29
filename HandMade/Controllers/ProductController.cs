using HandMade.Data;
using HandMade.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;


namespace HandMade.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db;
        public ProductController(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IActionResult> ListProducts(string searchString, string filter)
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


        public IActionResult AddProducts()
        {
         

            var categories = db.Categories
             .Where(c => c.ActiveCategories == "Активная") 
             .Select(s => new SelectOptions
             {
                 value = s.CategoriesId,
                 text = s.NameOfTheCategories
             })
             .ToList();

            TempData["categories"] = categories;

            return View();
        }
        public record SelectOptions
        {
            public int value { get; set; }
            public string text { get; set; }
        }

     
        [HttpPost]
          [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProducts(Products product, IFormFile imageFile)
         {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    

                        byte[] p1 = null;
                        using (var fs1 = imageFile.OpenReadStream())
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                        product.ImageProduct = p1;

                    
                }

                db.Add(product);
                await db.SaveChangesAsync();
                return RedirectToAction("ListProducts");
            }

            var categories = db.Categories
                .Select(s => new SelectOptions
                {
                    value = s.CategoriesId,
                    text = s.NameOfTheCategories
                })
                .ToList();
            TempData["categories"] = categories;
            return View(product);
        

          }
        public async Task<IActionResult> EditProducts(int? id)
        {


            if (id != null)
            {
                Products products = await db.Products.FirstOrDefaultAsync(p => p.ProductsId == id);
                var categories = db.Categories
                 .Select(s => new SelectOptions
                 {
                     value = s.CategoriesId,
                     text = s.NameOfTheCategories
                 })
             .ToList();
                TempData["categories"] = categories;

                if (products != null)
                    return View(products);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditProducts(int id, IFormFile image, Products model)
        {
            if (id != model.ProductsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var product = await db.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                if (image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        product.ImageProduct = memoryStream.ToArray();
                    }
                }

                product.NameOfTheProduct = model.NameOfTheProduct;
                product.Description = model.Description;
                product.Price = model.Price;
                product.Quantity = model.Quantity;
                product.CategoriesId = model.CategoriesId;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (!ProductsExists(model.ProductsId))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    //    throw;
                    //}
                }

                return RedirectToAction(nameof(ListProducts));
            }

            return View(model);
        }



        [HttpGet]
        [ActionName("DeleteProducts")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Products computers = await db.Products.FirstOrDefaultAsync(p => p.ProductsId == id);
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

                var product = await db.Products
               .Include(p => p.Carts)
               .SingleAsync(p => p.ProductsId == id);

                db.Carts.RemoveRange(product.Carts);
                db.Products.Remove(product);

                await db.SaveChangesAsync();
                return RedirectToAction("ListProducts");
            }
            return NotFound();
        }
    }
}
