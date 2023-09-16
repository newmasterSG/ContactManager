using ContactManager.Domain.Entities;
using ContactManager.Domain.Interfaces;
using ContactManaget.UI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactManaget.UI.Controllers
{
    public class AccountController : Controller
    {
        private ILogger<AccountController> _logger;
        private readonly EmailSender _emailSender;
        private readonly UserManager<UserEntity> _userManager;
        private readonly SignInManager<UserEntity> _signInManager;
        public AccountController(ILogger<AccountController> logger,
            UserManager<UserEntity> userManager,
            SignInManager<UserEntity> signInManager,
            EmailSender emailSender)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user.EmailConfirmed == false)
                {
                    ModelState.AddModelError("", $"User not confirmed email");
                    return View();
                }

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}
