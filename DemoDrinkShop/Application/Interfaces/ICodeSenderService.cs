namespace DemoDrinkShop.Application.Interfaces
{
    public interface ICodeSenderService
    {
        public Task SendCode(string emailTo, string code);
    }
}
