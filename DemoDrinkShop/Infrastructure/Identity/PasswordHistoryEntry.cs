using Microsoft.EntityFrameworkCore;

namespace DemoDrinkShop.Infrastructure.Identity
{
    [PrimaryKey(nameof(UserId), nameof(IterationId))]
    public class PasswordHistoryEntry
    {
        public int IterationId { get; set; }

        public string UserId { get; set; }

        public string PasswordHash { get; set; }
    }
}
