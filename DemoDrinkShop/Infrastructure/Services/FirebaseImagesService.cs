using DemoDrinkShop.Application.Interfaces;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace DemoDrinkShop.Infrastructure.Services
{
    public class FirebaseImagesService : IImageStorageService, IDisposable
    {
        private readonly string bucketName;
        private readonly StorageClient _storageClient;

        public FirebaseImagesService(IConfiguration configuration)
        {
            this.bucketName = configuration["Firebase:BucketName"];
            GoogleCredential serviceAccCredential;

            using (FileStream fs = new FileStream(configuration["Firebase:OauthKeyPath"], FileMode.Open, FileAccess.Read))
            {
                serviceAccCredential = GoogleCredential.FromStream(fs);
            }

            _storageClient = StorageClient.Create(serviceAccCredential);
        }
        public string GetEndURL(string name)
            => $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{name}?alt=media";
        public async Task DeleteImageFromDrive(string imageName)
        {
            await _storageClient.DeleteObjectAsync(bucketName, imageName);
        }

        public async Task<string> LoadProductImageToDrive(string fileName, string contentType, Stream str)
        {
            Google.Apis.Storage.v1.Data.Object meta = null;

            meta = await _storageClient.UploadObjectAsync(bucketName, fileName, contentType, str);

            await _storageClient.UpdateObjectAsync(meta);
            return meta.Name;
        }
        public async Task<bool> ImageExists(string imageName)
        {
            Google.Apis.Storage.v1.Data.Object? meta = null;
            try
            {
                meta = await _storageClient.GetObjectAsync(bucketName, imageName);
            }
            catch (GoogleApiException e)
            {
                return false;
            }
            return meta != null;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _storageClient.Dispose();
        }
    }
}
