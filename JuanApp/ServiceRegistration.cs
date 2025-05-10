using JuanApp.Data;
using JuanApp.Models;
using JuanApp.Services;
using JuanApp.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JuanApp
{
    public static class ServiceRegistration
    {
        public static void AddServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddControllersWithViews();



            services.AddDbContext<JuanDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.Configure<EmailSetting>(configuration.GetSection("Email"));
            services.AddScoped<LayoutService>();
            services.AddScoped<EmailService>();
            services.AddHttpContextAccessor();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.User.RequireUniqueEmail = true;
                opt.Lockout.MaxFailedAccessAttempts = 3;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opt.Lockout.AllowedForNewUsers = true;
                opt.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<JuanDbContext>().AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = options.Events.OnRedirectToLogin = context =>
                {
                    var uri = new Uri(context.RedirectUri);
                    if (context.Request.Path.Value.ToLower().StartsWith("/manage"))
                    {
                        context.Response.Redirect("/manage/account/login" + uri.Query);
                    }
                    else
                    {
                        context.Response.Redirect("/account/login" + uri.Query);
                    }
                    return Task.CompletedTask;
                };
            });
        }
    }
}
