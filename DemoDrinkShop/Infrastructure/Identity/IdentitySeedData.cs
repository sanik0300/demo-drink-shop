using Microsoft.AspNetCore.Identity;

namespace DemoDrinkShop.Infrastructure.Identity
{
    public class IdentitySeedData
    {
        private const string adminUser = "Admin";
        private const string adminPassword = "Secret123$";
        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                UserManager<ExtendedIdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ExtendedIdentityUser>>();
                ExtendedIdentityUser user = await userManager.FindByNameAsync(adminUser);
                if (user == null)
                {
                    user = new ExtendedIdentityUser() { UserName = "Admin", Email = "example@gmail.com", Address = "somewhere test" };

                    await userManager.CreateAsync(user, adminPassword);
                }
            }
        }
    }
}
