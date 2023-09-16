using ContactManager.Application.InterfacesServices;
using ContactManager.Application.Services;
using ContactManager.Domain.Entities;
using ContactManager.Domain.Interfaces;
using ContactManager.Infrastructure.Context;
using ContactManager.Infrastructure.Email;
using ContactManager.Infrastructure.Email.Options;
using ContactManager.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Net;
using System.Security.Claims;
using System.Text;
using static Dropbox.Api.Files.MediaInfo;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace ContactManaget.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
            var connectionString = configuration["ConnectionStrings:DefaultConnection"];
            Console.Out.WriteLineAsync(connectionString);
            builder.Services.AddDbContext<ContactDbContext>(options =>
                options.UseSqlServer(connectionString).EnableSensitiveDataLogging());

            builder.Services.AddControllersWithViews();

            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

            builder.Services.AddTransient<EmailSender, SmtpEmailSender>();
            builder.Services.AddScoped<IContactRepository, ContactRepository>();
            builder.Services.AddScoped<ICsvService, CsvService>();
            builder.Services.AddScoped<IContactService, ContactService>();

            //Config Sign Up Settings
            builder.Services.AddIdentity<UserEntity, IdentityRole>(options =>
            {
                options.User.AllowedUserNameCharacters += " ";
                options.SignIn.RequireConfirmedEmail = Boolean.Parse(configuration["RegistrationSettings:RequireConfirmedEmail"]);
                options.Password.RequireDigit = Boolean.Parse(configuration["RegistrationSettings:RequireDigit"]);
                options.Password.RequireLowercase = Boolean.Parse(configuration["RegistrationSettings:RequireLowercase"]);
                options.Password.RequireUppercase = Boolean.Parse(configuration["RegistrationSettings:RequireUppercase"]);
                options.Password.RequiredLength = int.Parse(configuration["RegistrationSettings:RequiredLength"]);
                options.Password.RequireNonAlphanumeric = Boolean.Parse(configuration["RegistrationSettings:RequireNonAlphanumeric"]);
            }).AddEntityFrameworkStores<ContactDbContext>().AddDefaultTokenProviders();

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddCookie(options =>
           {
               options.Cookie.HttpOnly = true;
               options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
               options.SlidingExpiration = true;
           })
           .AddJwtBearer(options =>
           {
               options.SaveToken = true;
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   RequireExpirationTime = Boolean.Parse(configuration["JwtSettings:RequireExpirationTime"]),
                   ValidateIssuer = Boolean.Parse(configuration["JwtSettings:ValidateIssuer"]),
                   ValidateAudience = Boolean.Parse(configuration["JwtSettings:ValidateAudience"]),
                   ValidateLifetime = Boolean.Parse(configuration["JwtSettings:ValidateLifetime"]),
                   ValidateIssuerSigningKey = Boolean.Parse(configuration["JwtSettings:ValidateIssuerSigningKey"]),
                   ValidIssuer = configuration["JwtSettings:Issuer"],
                   ValidAudience = configuration["JwtSettings:Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecurityKey"]))
               };
           });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<ContactDbContext>();

                dbContext.Database.EnsureCreated();

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();

                var roles = new[] { "Admin", "Manager", "Buyer" };

                string email = "testing.project.ts@gmail.com";
                string userPassword = "Eg.1234";

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                if (await userManager.FindByEmailAsync(email) == null)
                {
                    UserEntity adminUser = new UserEntity
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                    };

                    IdentityResult result = await userManager.CreateAsync(adminUser, userPassword);

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