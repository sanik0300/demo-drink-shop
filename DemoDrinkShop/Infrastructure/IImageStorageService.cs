namespace DemoDrinkShop.Infrastructure
{
    public interface IImageStorageService
    {
        string GetEndURL(string name);

        Task<bool> ImageExists(string imageName);

        Task<string> LoadProductImageToDrive(string fileName, string contentType, Stream str);

        Task DeleteImageFromDrive(string imageName);
    }
}
