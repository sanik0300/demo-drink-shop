using Microsoft.AspNetCore.Identity;

namespace DemoDrinkShop.Models
{
	public class IdentitySeedData
	{
		private const string adminUser = "Admin";
		private const string adminPassword = "Secret123$";
		public static async void EnsurePopulated(IApplicationBuilder app)
		{
			UserManager<IdentityUser> userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
			IdentityUser user = await userManager.FindByIdAsync(adminUser);
			if (user == null)
			{
				user = new IdentityUser("Admin");
				await userManager.CreateAsync(user, adminPassword);
			}
		}
	}
}
