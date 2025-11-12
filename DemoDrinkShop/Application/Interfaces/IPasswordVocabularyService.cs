namespace DemoDrinkShop.Application.Interfaces
{
    public interface IPasswordVocabularyService
    {
        string ComputeHash(string text);

        Task<bool> Contains(string hash);
    }
}
