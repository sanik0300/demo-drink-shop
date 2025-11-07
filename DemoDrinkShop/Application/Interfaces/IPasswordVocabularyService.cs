namespace DemoDrinkShop.Application.Interfaces
{
    public interface IPasswordVocabularyService
    {
        string ComputeHash(string text);

        Task<bool> IsToReject(string pass);
    }
}
