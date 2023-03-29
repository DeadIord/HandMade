using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using HandMade.Models;



namespace HandMade.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
       



        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users;
            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"Пользователь с идентификатором = {model.Id} не найден";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
           

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"Пользователь с идентификатором =  {id}  не найден";
                return View("NotFound");
            }

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                
                PhoneNumber = user.PhoneNumber,

                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);


        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"Пользователь с идентификатором =  {id}  не найден";
                return View("NotFound");
            }
            else
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    ViewBag.ErrorMessage = $"Вы не можете удалить пользователя-администратора";
                    return RedirectToAction("ListUsers");
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListUsers");
            }



            //var user = await _userManager.FindByIdAsync(id);

            //if (user == null)
            //{
            //    ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
            //    return View("NotFound");
            //}
            //else
            //{
            //    var result = await userManager.DeleteAsync(user);

            //    if (result.Succeeded)
            //    {
            //        return RedirectToAction("ListUsers");
            //    }

            //    foreach (var error in result.Errors)
            //    {
            //        ModelState.AddModelError("", error.Description);
            //    }

            //    return View("ListUsers");
            //}
        }
    }
}
