using DemoDrinkShop.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoDrinkShop.Models
{
	public class AppIdentityDbContext : IdentityDbContext<ExtendedIdentityUser>
	{
		public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) { }
	}
}
