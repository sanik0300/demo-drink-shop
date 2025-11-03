using DemoDrinkShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoDrinkShop.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
