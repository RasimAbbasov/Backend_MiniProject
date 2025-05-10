using JuanApp.Models;
using JuanApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using JuanApp.Services;
using Microsoft.CodeAnalysis.Options;
using JuanApp.Settings;
using Microsoft.Extensions.Options;

namespace JuanApp.Controllers
{
    public class AccountController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        EmailService emailService,
        IOptions<EmailSetting> emailOptions
        ) : Controller
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
            //send email verification
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = Url.Action("VerifyEmail", "Account", new { email = user.Email, token = token }, Request.Scheme);
            //create email
           
            using StreamReader streamReader = new StreamReader("wwwroot/templates/forgotpassword.html");
            string body = await streamReader.ReadToEndAsync();
            body = body.Replace("{{url}}", url);
            body = body.Replace("{{username}}", user.FullName);
           emailService.SendEmail(user.Email, "Email verification", body, emailOptions.Value);
            //send email
            
            return RedirectToAction("Login");
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(UserLoginVm userLoginVm, string? returnUrl)
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
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Email is not confirmed");
                return View();
            }
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
            Response.Cookies.Delete("basket");
            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Profile(string tab = "dashboard")
        {
            ViewBag.tab = tab;

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            UserUpdateProfileVm userUpdateProfileVm = new UserUpdateProfileVm
            {
                FullName = user.FullName,
                Username = user.UserName,
                Email = user.Email
            };
            UserProfileVm userProfileVm = new UserProfileVm
            {
                UserUpdateProfileVm = userUpdateProfileVm
            };
            return View(userProfileVm);
        }
        [Authorize(Roles = "Member")]
        [HttpPost]
        public async Task<IActionResult> Profile(UserUpdateProfileVm userUpdateProfileVm, string tab = "profile")
        {
            ViewBag.tab = tab;
            UserProfileVm userProfileVm = new UserProfileVm
            {
                UserUpdateProfileVm = userUpdateProfileVm
            };
            if (!ModelState.IsValid)
            {
                return View(userProfileVm);
            }
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            if (userUpdateProfileVm.NewPassword != null)
            {
                if (userUpdateProfileVm.CurrentPassword == null)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is required");
                    return View(userProfileVm);
                }
                else
                {
                    var passwordUpdateResult = await userManager.ChangePasswordAsync(user, userUpdateProfileVm.CurrentPassword, userUpdateProfileVm.NewPassword);
                    if (!passwordUpdateResult.Succeeded)
                    {
                        foreach (var error in passwordUpdateResult.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(userProfileVm);
                    }
                }
            }
            user.FullName = userUpdateProfileVm.FullName;
            user.UserName = userUpdateProfileVm.Username;
            user.Email = userUpdateProfileVm.Email;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(userProfileVm);
            }
            await signInManager.SignInAsync(user, true);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVm forgotPasswordVm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await userManager.FindByEmailAsync(forgotPasswordVm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email is incorrect");
                return View();
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("ResetPassword", "Account", new { email = user.Email, token = token }, Request.Scheme);
            //create email
            
            using StreamReader streamReader = new StreamReader("wwwroot/templates/forgotpassword.html");
            string body = await streamReader.ReadToEndAsync();

            body = body.Replace("{{url}}", url);
            body = body.Replace("{{username}}", user.FullName);
            emailService.SendEmail(user.Email, "Reset password", body, emailOptions.Value);
            //send email
            
            return RedirectToAction("Index", "Home");
        }
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVm resetPasswordVm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await userManager.FindByEmailAsync(resetPasswordVm.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email not found");
                return View();
            }
            var result = await userManager.ResetPasswordAsync(user, resetPasswordVm.Token, resetPasswordVm.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }
            return RedirectToAction("Login");
        }
    }
}
