using DemoDrinkShop.Application;
using DemoDrinkShop.Application.Interfaces;
using DemoDrinkShop.Infrastructure.Identity;
using DemoDrinkShop.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace DemoDrinkShop.Presentation.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private static readonly string[] entryPurposes = { "login", "register", "code" };

        private readonly UserManager<ExtendedIdentityUser> userManager;
        private readonly SignInManager<ExtendedIdentityUser> signInManager;
        private readonly IMemoryCache memoryCache;
        
        private readonly IPasswordVocabularyService passwordService;
        private readonly ICodeSenderService codeService;
        private readonly IVerificationCodeRepository codeRepository;


        public AccountController(IServiceProvider serviceProvider, IMemoryCache memoryCache, IVerificationCodeRepository codeRepository)
        {
            userManager = serviceProvider.GetRequiredService<UserManager<ExtendedIdentityUser>>();
            signInManager = serviceProvider.GetRequiredService<SignInManager<ExtendedIdentityUser>>();

            passwordService = serviceProvider.GetRequiredService<IPasswordVocabularyService>();
            codeService = serviceProvider.GetRequiredService<ICodeSenderService>();

            this.codeRepository = codeRepository;
            this.memoryCache = memoryCache;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Entry(string purpose, string returnUrl)
        {
            if (!entryPurposes.Contains(purpose))
            {
                return NotFound();
            }

            Debug.WriteLine(Request);

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
                ModelState.AddModelError("", "Invalid email/phone or password too short");
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
            bool thatPwd = await userManager.CheckPasswordAsync(user, loginModel.Password);
            if (!thatPwd)
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
                UserName = regModel.Name ?? (regModel.Email!= null? regModel.Email.Split('@')[0] : regModel.Phone),
            };
            var result = await userManager.CreateAsync(registered, regModel.Password);

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

        [HttpGet]
        [AllowAnonymous]
        public async Task<string> GetMyEmail()
        {
            IdentityUser? crtUser = await userManager.GetUserAsync(HttpContext.User);
            if(crtUser == null) { return string.Empty; }
            return crtUser.Email ?? string.Empty;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendVerificationCode([FromForm] string emailTo)
        {
            string cacheKey = $"code_{emailTo}";
            if(memoryCache.TryGetValue(cacheKey, out var cache))
            {
                int secondsLeft = 30 - (DateTime.UtcNow - (DateTime)cache).Seconds;

                return StatusCode(StatusCodes.Status429TooManyRequests, $"Wait {secondsLeft} more seconds before resending code");
            }

            int code = VerificationCode.GenerateValue();
            await codeService.SendCode(emailTo, code.ToString());
            Debug.WriteLine($"Code is {code}");

            memoryCache.Set(cacheKey, DateTime.UtcNow, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));

            VerificationCode codeEntity = new VerificationCode() { Email = emailTo, Value = code, ExpiresAt = DateTime.UtcNow.AddMinutes(15) };
            
            if(await codeRepository.GetByEmail(emailTo) != null)
            {
                await codeRepository.Save(codeEntity);
            }
            else {
                await codeRepository.Add(codeEntity);
            }

            return Ok();
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyPasswordChange([FromForm] PasswordChangeViewModel viewModel)
        {
            VerificationCode? codeEntity = await codeRepository.GetByEmail(viewModel.Email);

            if(codeEntity == null)
            {
                return NotFound("Password code for this email not found");
            }
            if(viewModel.Code != codeEntity.Value)
            {
                return Unauthorized("Wrong code value");
            }
            if(codeEntity.IsExpired)
            {
                return Unauthorized("This code is expired already");
            }

            ExtendedIdentityUser me = await userManager.FindByEmailAsync(viewModel.Email);

            string token = await userManager.GeneratePasswordResetTokenAsync(me);
            await userManager.ResetPasswordAsync(me, token, viewModel.NewPassword);
                        
            return Redirect("/");
        }
    }
}
