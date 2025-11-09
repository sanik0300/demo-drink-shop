using DemoDrinkShop.Application.Interfaces;
using DemoDrinkShop.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace DemoDrinkShop.Infrastructure.Repositories
{
    public class EFPasswordHistoryRepository : IPasswordHistoryRepository
    {
        private readonly AppIdentityDbContext context;

        public EFPasswordHistoryRepository(AppIdentityDbContext context) => this.context = context;

        public async Task AddEntry(PasswordHistoryEntry entry)
        {
            await context.PasswordsHistory.AddAsync(entry);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PasswordHistoryEntry>> GetForUser(string userId)
        {
            return await context.PasswordsHistory.Where(x => x.UserId == userId).ToListAsync();
        }
    }
}
