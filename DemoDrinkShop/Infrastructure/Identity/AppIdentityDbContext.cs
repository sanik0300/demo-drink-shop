using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DemoDrinkShop.Infrastructure.Identity
{
    public class AppIdentityDbContext : IdentityDbContext<ExtendedIdentityUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) { }

        public DbSet<UselessPassword> UselessPasswords { get; set; }
    }
}
