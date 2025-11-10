using DemoDrinkShop.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DemoDrinkShop.Infrastructure.Identity
{
    public class EFVerificationCodeRepository : IVerificationCodeRepository
    {
        private readonly AppIdentityDbContext context;

        public EFVerificationCodeRepository(AppIdentityDbContext context) => this.context = context;

        public async Task Add(VerificationCode entity)
        {
            context.VerificationCodes.Add(entity);
            await context.SaveChangesAsync();
        }

        public async Task Save(VerificationCode entity)
        {
            VerificationCode existing = await context.VerificationCodes.FirstAsync(c => c.Email == entity.Email);

            existing.Value = entity.Value;
            existing.Used = entity.Used;
            existing.ExpiresAt = entity.ExpiresAt;
            await context.SaveChangesAsync();
        }

        public async Task<VerificationCode?> GetByEmail(string email)
        {
            return await context.VerificationCodes.FirstOrDefaultAsync(c => c.Email == email);
        }
        public async Task CleanExpired()
        {
            IEnumerable<VerificationCode> codes = (await context.VerificationCodes.ToListAsync()).Where(c => c.IsExpired);
            context.VerificationCodes.RemoveRange(codes);
            await context.SaveChangesAsync();
        }
    }
}
