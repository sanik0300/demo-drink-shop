using DemoDrinkShop.Application;
using DemoDrinkShop.Application.Interfaces;
using DemoDrinkShop.Infrastructure.Identity;
using DemoDrinkShop.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoDrinkShop.Presentation.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ExtendedIdentityUser> userManager;
        private readonly SignInManager<ExtendedIdentityUser> signInManager;
        private readonly IPasswordHasher<ExtendedIdentityUser> passwordHasher;
        private readonly IPasswordVocabularyService passwordService;

        public AccountController(UserManager<ExtendedIdentityUser> userMgr, SignInManager<ExtendedIdentityUser> signInMgr,
                                 IPasswordHasher<ExtendedIdentityUser> hasher, IPasswordVocabularyService hashingService)
        {
            userManager = userMgr;
            signInManager = signInMgr;
            passwordHasher = hasher;
            this.passwordService = hashingService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Entry(string purpose, string returnUrl)
        {
            if (purpose != "login" && purpose != "register")
            {
                return NotFound();
            }

            ViewBag.Purpose = purpose;
            return View(new UserViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [PhoneNumberResourceFilter]
        [ModelErrorsSurfacingFilter]
        public async Task<IActionResult> Login([FromForm] UserViewModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Invalid email/phone or password too long");
                ViewBag.Purpose = "login";
                return View("Entry");
            }

            ExtendedIdentityUser? user = null;
            if (!string.IsNullOrEmpty(loginModel.Email))
            {
                user = await userManager.FindByEmailAsync(loginModel.Email);
            }
            if (user == null)
            {
                user = await userManager.Users.Where(u => u.PhoneNumber == loginModel.Phone).FirstOrDefaultAsync();
            }

            if (user == null)
            {
                ModelState.AddModelError("", "Account not found");
                ViewBag.Purpose = "login";
                return View("Entry");
            }
            if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginModel.Password) == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Wrong password");
                ViewBag.Purpose = "login";
                return View("Entry");
            }

            await signInManager.SignOutAsync();
            if ((await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false)).Succeeded)
            {
                return Redirect(loginModel?.ReturnUrl ?? "/Product/List");
            }

            ModelState.AddModelError("", "Failed to sign in, try again");
            ViewBag.Purpose = "login";
            return View("Entry");
        }

        [HttpPost]
        [AllowAnonymous]
        [PhoneNumberResourceFilter]
        [ModelErrorsSurfacingFilter]
        public async Task<IActionResult> Register([FromForm] UserViewModel regModel)
        {
            IdentityUser? existingUsr;
            if (regModel.Email != null)
            {
                existingUsr = await userManager.FindByEmailAsync(regModel.Email);
                if (existingUsr != null)
                {
                    ModelState.AddModelError("", "User with such email already exists");
                    return View("Entry");
                }
            }
            if (regModel.Phone != null)
            {
                existingUsr = await userManager.Users.Where(u => u.PhoneNumber == regModel.Phone).FirstOrDefaultAsync();
                if (existingUsr != null)
                {
                    ModelState.AddModelError("", "User with such phone number already exists");
                    return View("Entry");
                }
            }

            if(await passwordService.IsToReject(regModel.Password))
            {
                ModelState.AddModelError("", "Password from a prohibited list");
                return View("Entry");
            }

            ExtendedIdentityUser registered = new ExtendedIdentityUser()
            {
                Address = regModel.Address,
                Email = regModel.Email,
                PhoneNumber = regModel.Phone,
                VerifyByEmail = !regModel.VerifyByEmail.GetValueOrDefault(),
                UserName = regModel.Name
            };
            registered.PasswordHash = passwordHasher.HashPassword(registered, regModel.Password);

            await userManager.CreateAsync(registered);

            if (signInManager.IsSignedIn(HttpContext.User))
            {
                await signInManager.SignOutAsync();
            }
            await signInManager.PasswordSignInAsync(registered, regModel.Password, false, false);

            return Redirect(regModel?.ReturnUrl ?? "/Product/List");
        }

        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
    }

}
