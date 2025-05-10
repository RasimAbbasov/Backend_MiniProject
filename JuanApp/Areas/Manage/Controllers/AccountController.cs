using JuanApp.Areas.Manage.ViewModel;
using JuanApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JuanApp.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class AccountController(
          UserManager<AppUser> userManager,
          SignInManager<AppUser> signInManager,
          RoleManager<IdentityRole> roleManager
          ) : Controller
    {
        public async Task<IActionResult> CreateAdmin()
        {
            AppUser user = new()
            {
                UserName = "admin",
                FullName = "Admin",
                Email = "admin@gmail.com",
            };
            var result = await userManager.CreateAsync(user, "_Admin123");
            await userManager.AddToRoleAsync(user, "Admin");

            return Json(result);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(AdminLoginVm adminLoginVm,string? returnUrl)
        {
            if (!ModelState.IsValid)
                return View();
            var user = await userManager.FindByNameAsync(adminLoginVm.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View();
            }
            if (user==null||(!(await userManager.IsInRoleAsync(user,"Admin")||!await userManager.IsInRoleAsync(user,"SuperAdmin"))))
            {
                ModelState.AddModelError("", "You are not allowed to login");
                return View();
            }
            var result = await userManager.CheckPasswordAsync(user, adminLoginVm.Password);
            if (!result)
            {
                ModelState.AddModelError("", "Username or password is incorrect");
                return View();
            }
            await signInManager.SignInAsync(user, false);
            //var result = await signInManager.CheckPasswordSignInAsync(user, adminLoginVm.Password, false);
            //if(!result.Succeeded)
            //{
            //    ModelState.AddModelError("", "Your account is locked out");
            //    return View();
            //}

            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("Index", "Dashboard");
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", "Manage");
        }
        [Authorize(Roles ="Admin,SuperAdmin")]
        public async Task<IActionResult> UserProfile()
        {
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            return Json(user);
        }
        public async Task<IActionResult> CreateRole()
        {
            await roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            await roleManager.CreateAsync(new IdentityRole { Name = "Member" });
            await roleManager.CreateAsync(new IdentityRole { Name = "SuperAdmin" });
            return Content("Role created");
        }
    }
}
