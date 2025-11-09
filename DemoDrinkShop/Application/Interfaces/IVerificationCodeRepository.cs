using DemoDrinkShop.Infrastructure.Identity;

namespace DemoDrinkShop.Application.Interfaces
{
    public interface IVerificationCodeRepository
    {
        Task<VerificationCode?> GetByEmail(string email);
        Task Add(VerificationCode entity);
        Task Save(VerificationCode entity);

        Task CleanExpired();
    }
}
