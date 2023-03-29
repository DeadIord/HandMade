using HandMade.Data;
using HandMade.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HandMade.Controllers
{
    public class CartsController : Controller
    {
        private ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        public CartsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Cart()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Отфильтровать товары, чтобы выбрать только те, у которых ApplicationUserId совпадает с Id текущего пользователя      
            if(currentUser != null)
            {
                var cartItems = await db.Carts
              .Include(c => c.Products)
              .Where(c => c.ApplicationUser.Id == currentUser.Id)
              .ToListAsync();

                return View(cartItems);
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            // Получаем текущего пользователя
            var user = await _userManager.GetUserAsync(HttpContext.User);

            // Получаем товар по идентификатору
            var product = await db.Products.FindAsync(id);

            // Создаем или обновляем запись в корзине пользователя
            var cart = await db.Carts
                .Where(c => c.ApplicationUser.Id == user.Id && c.Products.ProductsId == product.ProductsId)
                .FirstOrDefaultAsync();

            if (cart == null)
            {
                cart = new Carts
                {
                    ApplicationUser = user,
                    Products = product,
                    Quantity = 1
                };
                db.Carts.Add(cart);
            }
            else
            {
                cart.Quantity++;
                db.Carts.Update(cart);
            }

            await db.SaveChangesAsync();

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int ProductsId, int quantity)
        {
            // Получить текущего пользователя
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Создать объект корзины и заполнить его свойства
            var cartItem = new Carts
            {
                Quantity = quantity,
                Products = await db.Products.FindAsync(ProductsId),
                ApplicationUser = currentUser
            };

            // Добавить объект корзины в базу данных и сохранить изменения
            db.Carts.Add(cartItem);
            await db.SaveChangesAsync();

            // Перенаправить пользователя на страницу корзины
            return RedirectToAction("Cart");
        }
        [HttpGet]
        [ActionName("DeleteCarts")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                Carts carts = await db.Carts.FirstOrDefaultAsync(p => p.CartsId == id);
                if (carts != null)
                    return View(carts);
            }
            return NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int? CartsId)
        {
            if (CartsId != null)
            {
                Carts carts = new() { CartsId = CartsId.Value };
                db.Entry(carts).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Cart");
            }
            return NotFound();
        }
    }
}
