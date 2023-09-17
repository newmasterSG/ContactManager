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
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ContactManaget.UI.Config
{
    public static class MyServicesCollectionExtention
    {
        public static IServiceCollection AddMyService(this IServiceCollection services)
        {
            services.AddTransient<EmailSender, SmtpEmailSender>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<ICsvService, CsvService>();
            services.AddScoped<IContactService, ContactService>();

            return services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<UserEntity, IdentityRole>(options =>
            {
                options.User.AllowedUserNameCharacters += " ";
                options.SignIn.RequireConfirmedEmail = Boolean.Parse(configuration["RegistrationSettings:RequireConfirmedEmail"]);
                options.Password.RequireDigit = Boolean.Parse(configuration["RegistrationSettings:RequireDigit"]);
                options.Password.RequireLowercase = Boolean.Parse(configuration["RegistrationSettings:RequireLowercase"]);
                options.Password.RequireUppercase = Boolean.Parse(configuration["RegistrationSettings:RequireUppercase"]);
                options.Password.RequiredLength = int.Parse(configuration["RegistrationSettings:RequiredLength"]);
                options.Password.RequireNonAlphanumeric = Boolean.Parse(configuration["RegistrationSettings:RequireNonAlphanumeric"]);
            }).AddEntityFrameworkStores<ContactDbContext>().AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
                services.AddAuthentication(option =>
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

            return services;
        }

        public static IServiceCollection AddOwnDependency(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmtpOptions>(configuration.GetSection("Smtp"));
            services.AddMyService();
            services.AddIdentity(configuration);
            services.AddAuth(configuration);

            return services;
        }
    }
}
