
using DemoDrinkShop.Application.Interfaces;

namespace DemoDrinkShop.Infrastructure.Services
{
    public class ExpiredRecoveryCodesCleanupService : BackgroundService
    {
        private readonly TimeSpan refreshInterval;
        private readonly IServiceScopeFactory serviceFactory;

        public ExpiredRecoveryCodesCleanupService(IServiceScopeFactory serviceFactory, IConfiguration conf)
        {
            this.serviceFactory = serviceFactory;

            byte minutes = 15;
            byte.TryParse(conf["RecoveryCodeMinutes"], out minutes);
            refreshInterval = TimeSpan.FromMinutes(minutes);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (PeriodicTimer timer = new PeriodicTimer(refreshInterval))
            {
                while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
                {
                    using (IServiceScope scope = serviceFactory.CreateScope())
                    {
                        IVerificationCodeRepository codeRepo = scope.ServiceProvider.GetRequiredService<IVerificationCodeRepository>();
                        await codeRepo.CleanExpired();
                    }
                }
            }
        }
    }
}
