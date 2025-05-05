using JuanApp.Models;
using JuanApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JuanApp.Controllers
{
    public class AccountController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager) : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterVm userRegisterVm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser user = await userManager.FindByNameAsync(userRegisterVm.Username);
            if (user != null)
            {
                ModelState.AddModelError("Username", "This username already exists");
                return View();
            }
            user = new AppUser
            {
                UserName = userRegisterVm.Username,
                FullName = userRegisterVm.FullName,
                Email = userRegisterVm.Email
            };
            var result = await userManager.CreateAsync(user, userRegisterVm.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            await userManager.AddToRoleAsync(user, "Member");
            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginVm userLoginVm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await userManager.FindByNameAsync(userLoginVm.UserNameOrEmail);
            if (user == null)
            {
                user = await userManager.FindByEmailAsync(userLoginVm.UserNameOrEmail);
                if (user == null)
                {
                    ModelState.AddModelError("", "Username or email is incorrect");
                    return View();
                }
            }
            if (!await userManager.IsInRoleAsync(user, "Member"))
            {
                ModelState.AddModelError("", "You are not allowed to login");
                return View();
            }
            var result = await signInManager.PasswordSignInAsync(user, userLoginVm.Password, userLoginVm.RememberMe, false);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Account lockedout for 5 min");
                return View();
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Password is incorrect");
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
