using ContactManager.Domain.Entities;
using ContactManager.Infrastructure.Context;
using ContactManaget.UI.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System.Security.Claims;
using TelegramSink;

namespace ContactManaget.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            //I am using user secret for telegram bot, u can you own bot with chatID
            Log.Logger = new LoggerConfiguration().MinimumLevel.Override("Microsoft", LogEventLevel.Error)
           .MinimumLevel.Override("System", LogEventLevel.Error)
           .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Error)
           .WriteTo.TeleSink(configuration["TelegramBot:BotToken"],
           configuration["TelegramBot:ChatID"])
           .CreateLogger();

            var connectionString = configuration["ConnectionStrings:DefaultConnection"];
            builder.Services.AddDbContext<ContactDbContext>(options =>
                options.UseSqlServer(connectionString).EnableSensitiveDataLogging());

            builder.Services.AddControllersWithViews();

            builder.Services.AddOwnDependency(configuration);

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.Use(async (context, next) =>
            {
                var token = context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(token))
                    context.Request.Headers.Add("Authorization", "Bearer " + token);

                await next();
            });

            app.UseStaticFiles();

            //Seeding Data
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<ContactDbContext>();

                dbContext.Database.EnsureCreated();

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();

                var roles = new[] { "Admin", "Manager", "Buyer" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                if (await userManager.FindByEmailAsync(configuration["DefaultUser:email"]) == null)
                {
                    UserEntity adminUser = new UserEntity
                    {
                        UserName = configuration["DefaultUser:email"],
                        Email = configuration["DefaultUser:password"],
                        EmailConfirmed = Boolean.TryParse(configuration["DefaultUser:emailConfirmed"], out bool p),
                    };

                    IdentityResult result = await userManager.CreateAsync(adminUser, configuration["DefaultUser:password"]);

                    if (result.Succeeded)
                    {
                        await userManager.AddClaimAsync(adminUser, new Claim(ClaimTypes.Name, adminUser.UserName));
                    }
                }
                dbContext.SaveChanges();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}