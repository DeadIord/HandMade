using HandMade.Data;
using HandMade.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static HandMade.Controllers.ProductController;
using System.Security.Claims;

namespace HandMade.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> OrdersViewAsync()
        {
            var product = db.Orders.Include(p => p.OrderDetails).AsQueryable();

            return View( await product.ToListAsync());
        }

        public async Task<IActionResult> OrdersDetail(int? id)
        {
            var order = await db.Orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).FirstOrDefaultAsync(o => o.OrdersId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order.OrderItems.Select(oi => oi.Product));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeOrderStatus(int orderId, string newStatus)
        {
            var order = await db.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = newStatus;
            await db.SaveChangesAsync();

            return RedirectToAction("OrdersView");
        }


        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderViewModel orderViewModel)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            var cartItems = await db.Carts
                .Include(c => c.Products)
                .Where(c => c.ApplicationUser.Id == currentUser.Id)
                .ToListAsync();

            if (cartItems.Count == 0)
            {
                ModelState.AddModelError("", "Корзина пуста");
            }

            if (ModelState.IsValid)
            {
                var order = new Orders
                {
                    OrderNo = Guid.NewGuid().ToString(),
                    ApplicationUserId = currentUser.Id,
                    OrderDate = DateTime.Now,
                    Status = "В обработке",
                    OrderDetails = new OrderDetails
                    {
                        FirstName = orderViewModel.FirstName,
                        LastName = orderViewModel.LastName,
                        Patronymic = orderViewModel.Patronymic,
                        NumberPhone = orderViewModel.NumberPhone,
                        Addres = orderViewModel.Addres
                    }
                };

                foreach (var cartItem in cartItems)
                {
                    var orderProduct = new OrderItem
                    {
                        ProductsId = cartItem.Products.ProductsId,
                        Quantity = cartItem.Quantity,
                        Orders = order
                    };

                    db.OrderItem.Add(orderProduct);
                    db.Carts.Remove(cartItem);
                }

                await db.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(orderViewModel);
        }
    }
}