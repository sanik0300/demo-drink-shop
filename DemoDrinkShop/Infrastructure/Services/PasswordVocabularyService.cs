using DemoDrinkShop.Application.Interfaces;
using DemoDrinkShop.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DemoDrinkShop.Infrastructure.Services
{
    public class PasswordVocabularyService : IPasswordVocabularyService
    {
        private static readonly byte[] _pepperBytes;
        private readonly AppIdentityDbContext identityDbContext;
        static PasswordVocabularyService()
        {
            string pepper = File.ReadAllText("Credentials/pepper.txt");
            _pepperBytes = Encoding.UTF8.GetBytes(pepper);
        }
        public PasswordVocabularyService(AppIdentityDbContext identityDbContext) => this.identityDbContext = identityDbContext;

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            using (IServiceScope scope = app.ApplicationServices.CreateScope())
            {
                AppIdentityDbContext idContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();

                if (await idContext.UselessPasswords.AnyAsync()) { return; }

                string[] badPasswords = await File.ReadAllLinesAsync("useless_passwords_vocabulary.txt");
                IPasswordVocabularyService hash_service = scope.ServiceProvider.GetRequiredService<IPasswordVocabularyService>();

                IEnumerable<UselessPassword> pwds = badPasswords.Select(s => new UselessPassword(hash_service.ComputeHash(s)));

                await idContext.UselessPasswords.AddRangeAsync(pwds);
                await idContext.SaveChangesAsync();
            }
        }

        public string ComputeHash(string text)
        {
            string hex;
            using (HMACSHA256 hmac = new HMACSHA256(_pepperBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(text));
                hex = BitConverter.ToString(hashBytes);
            }
            return hex;
        }

        public async Task<bool> IsToReject(string pass)
        {
            string hashedStr = ComputeHash(pass);
            return await identityDbContext.UselessPasswords.AnyAsync(p => p.HashedValue == hashedStr);
        }
    }
}
