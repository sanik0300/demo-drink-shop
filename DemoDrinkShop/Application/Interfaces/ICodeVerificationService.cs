namespace DemoDrinkShop.Application.Interfaces
{
    public interface ICodeVerificationService
    {
        public Task SendCode(string emailTo, string code);
    }
}
