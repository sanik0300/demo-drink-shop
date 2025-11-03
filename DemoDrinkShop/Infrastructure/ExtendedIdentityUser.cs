using Microsoft.AspNetCore.Identity;

namespace DemoDrinkShop.Infrastructure
{
    public class ExtendedIdentityUser : IdentityUser
    {
        public string? Address { get; set; }
        public bool VerifyByEmail { get; set; }
    }
}
