using DemoDrinkShop.Infrastructure.Identity;

namespace DemoDrinkShop.Application.Interfaces
{
    public interface IPasswordHistoryRepository
    {
        Task<IEnumerable<PasswordHistoryEntry>> GetForUser(string userId);
        Task AddEntry(PasswordHistoryEntry entry);
    }
}
