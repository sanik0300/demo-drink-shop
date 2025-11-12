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
        private readonly IPasswordHistoryRepository historyRepository;

        private readonly byte recoveryCodeMinutes = 15;

        public AccountController(IServiceProvider serviceProvider, IMemoryCache memoryCache, IConfiguration configuration,
                                 IVerificationCodeRepository codeRepository, IPasswordHistoryRepository historyRepository)
        {
            userManager = serviceProvider.GetRequiredService<UserManager<ExtendedIdentityUser>>();
            signInManager = serviceProvider.GetRequiredService<SignInManager<ExtendedIdentityUser>>();

            passwordService = serviceProvider.GetRequiredService<IPasswordVocabularyService>();
            codeService = serviceProvider.GetRequiredService<ICodeSenderService>();

            this.codeRepository = codeRepository;
            this.memoryCache = memoryCache;
            this.historyRepository = historyRepository;

            byte.TryParse(configuration["RecoveryCodeMinutes"], out this.recoveryCodeMinutes);
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
        //[ModelErrorsSurfacingFilter]
        public async Task<IActionResult> Login([FromForm] UserViewModel loginModel)
        {
            if (!ModelState.IsValid)
            {           
                //ViewBag.Purpose = "login";
                return BadRequest("Invalid email/phone or password too short");
            }

            ExtendedIdentityUser? user = null;
            if (!string.IsNullOrEmpty(loginModel.Email))
            {
                user = await userManager.FindByEmailAsync(loginModel.Email);
            }
            if (user == null && !string.IsNullOrEmpty(loginModel.Phone))
            {
                user = await userManager.Users.Where(u => u.PhoneNumber == loginModel.Phone).FirstOrDefaultAsync();
            }

            if (user == null)
            {
                //ViewBag.Purpose = "login";
                return NotFound("Account not found");
            }
            bool thatPwd = await userManager.CheckPasswordAsync(user, loginModel.Password);
            if (!thatPwd)
            {
                //ViewBag.Purpose = "login";
                return NotFound("Wrong password");
            }

            await signInManager.SignOutAsync();
            if ((await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false)).Succeeded)
            {
                return Redirect(loginModel?.ReturnUrl ?? "/Product/List");
            }

            //ViewBag.Purpose = "login";
            return Unauthorized("Failed to sign in, try again");
        }

        [HttpPost]
        [AllowAnonymous]
        [PhoneNumberResourceFilter]
        //[ModelErrorsSurfacingFilter]
        public async Task<IActionResult> Register([FromForm] UserViewModel regModel)
        {
            IdentityUser? existingUsr;
            if (regModel.Email != null)
            {
                existingUsr = await userManager.FindByEmailAsync(regModel.Email);
                if (existingUsr != null)
                {
                    return Conflict("User with such email already exists");
                }
            }
            if (regModel.Phone != null)
            {
                existingUsr = await userManager.Users.Where(u => u.PhoneNumber == regModel.Phone).FirstOrDefaultAsync();
                if (existingUsr != null)
                {
                    return Conflict("User with such phone number already exists");
                }
            }

            if(await passwordService.Contains(passwordService.ComputeHash(regModel.Password)))
            {
                return UnprocessableEntity("Password from a prohibited list");
            }

            ExtendedIdentityUser registered = new ExtendedIdentityUser()
            {
                Address = regModel.Address,
                Email = regModel.Email,
                PhoneNumber = regModel.Phone,
                VerifyByEmail = !regModel.VerifyByEmail.GetValueOrDefault(),
                UserName = regModel.Name ?? (regModel.Email!= null? regModel.Email.Split('@')[0] : regModel.Phone),
            };
            await userManager.CreateAsync(registered, regModel.Password);

            PasswordHistoryEntry myFirstEntry = new PasswordHistoryEntry() 
            { 
                IterationId =0, 
                UserId=registered.Id,
                PasswordHash = passwordService.ComputeHash(regModel.Password),
            };
            await historyRepository.AddEntry(myFirstEntry);

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
            //await codeService.SendCode(emailTo, code.ToString());
            Debug.WriteLine($"email code is {code}");

            memoryCache.Set(cacheKey, DateTime.UtcNow, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));

            VerificationCode codeEntity = new VerificationCode() {
                Email = emailTo, Value = code, ExpiresAt = DateTime.UtcNow.AddMinutes(recoveryCodeMinutes) 
            };
            
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
            if(codeEntity.Used)
            {
                return UnprocessableEntity("This code is used already");
            }
            if(codeEntity.IsExpired)
            {
                return UnprocessableEntity("This code is expired already");
            }

            ExtendedIdentityUser me = await userManager.FindByEmailAsync(viewModel.Email);

            string myHash = passwordService.ComputeHash(viewModel.NewPassword);
            if(await passwordService.Contains(myHash))
            {
                return UnprocessableEntity("Password from a prohibited list");
            }

            IEnumerable<PasswordHistoryEntry> myHistory = await historyRepository.GetForUser(me.Id);

            if(myHistory.Any(p => p.PasswordHash==myHash))
            {
                return Unauthorized("You have already had this password before");
            }
            
            PasswordHistoryEntry updEntry = new PasswordHistoryEntry()
            {
                IterationId = myHistory.Count(),
                UserId = me.Id,
                PasswordHash = myHash
            };
            await historyRepository.AddEntry(updEntry);

            string token = await userManager.GeneratePasswordResetTokenAsync(me);
            await userManager.ResetPasswordAsync(me, token, viewModel.NewPassword);

            codeEntity.Used = true;
            await codeRepository.Save(codeEntity);
            
            return Redirect("/");
        }
    }
}
