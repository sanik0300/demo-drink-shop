using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Net;

using DemoDrinkShop.Infrastructure;
using Google;
namespace DrinkShopTests
{
    public sealed class MyImageServiceTest : StorageRelatedTest
    {
        private static Type[] typesForConstructor = new[] { typeof(string), typeof(string) };
        private IImageStorageService imagesService;

        public static IEnumerable<object[]> GetStorageTypesTested()
        {
            yield return new object[] { typeof(FirebaseImagesService) };
        }

        private IImageStorageService GetServiceInstanceWithReflection(Type type)
        {
            ConstructorInfo ctor = type.GetConstructor(typesForConstructor);
            return (IImageStorageService)ctor.Invoke(new object[] { bucketName, firebaseAuthPath });
        }

        [Fact]
        public void UrlFirebaseTest()
        {
            string expected = $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{testImgName}?alt=media";
            imagesService = new FirebaseImagesService(bucketName, firebaseAuthPath);

            string resultUrl = imagesService.GetEndURL(testImgName);

            Assert.Equal(expected, resultUrl);  
        }

        [Theory]
        [MemberData(nameof(GetStorageTypesTested))]
        public async Task ImageExistsPositiveTest(Type testedType)
        {
            imagesService = GetServiceInstanceWithReflection(testedType);
            string resultName;

            using (var fileStream = File.OpenRead(demoImgPath))
            {
                Google.Apis.Storage.v1.Data.Object uploadResult 
                    = await _storageClient.UploadObjectAsync(bucketName, testImgName, contentTypeStr, fileStream);
                resultName = uploadResult.Name;

                await _storageClient.UpdateObjectAsync(uploadResult);
            }

            bool exists = await imagesService.ImageExists(resultName);

            await _storageClient.DeleteObjectAsync(bucketName, testImgName);

            Assert.Equal(testImgName, resultName);
            Assert.True(exists);
        }

        [Theory]
        [MemberData(nameof(GetStorageTypesTested))]
        public async Task ImageExistsNegativeTest(Type testedType)
        {
            imagesService = GetServiceInstanceWithReflection(testedType);
            bool exists = await imagesService.ImageExists("nonexistent");
            Assert.False(exists);
        }


        [Theory]
        [MemberData(nameof(GetStorageTypesTested))]
        public async Task LoadingImageTest(Type testedType)
        {
            imagesService = GetServiceInstanceWithReflection(testedType);
            MemoryStream imageMemStream1 = new MemoryStream();

            Exception? loadExc;
            byte[] bytesIn;
            using (var fileStream = File.OpenRead(demoImgPath))
            {
                loadExc = await Record.ExceptionAsync(() => imagesService.LoadProductImageToDrive(testImgName, contentTypeStr, fileStream));

                fileStream.Position = 0;
                await fileStream.CopyToAsync(imageMemStream1);
                bytesIn = imageMemStream1.ToArray();
            }

            await imageMemStream1.DisposeAsync();

            byte[] bytesOut;
            using(MemoryStream imageMemStream2 = new MemoryStream())
            {
                await _storageClient.DownloadObjectAsync(bucketName, testImgName, imageMemStream2);
                bytesOut = imageMemStream2.ToArray();
            }

            await _storageClient.DeleteObjectAsync(bucketName, testImgName);
            

            Assert.Null(loadExc);
            Assert.True(bytesIn.SequenceEqual(bytesOut));
        }


        [Theory]
        [MemberData(nameof(GetStorageTypesTested))]
        public async Task DeletingImageTest(Type testedType)
        {
            imagesService = GetServiceInstanceWithReflection(testedType);
            using (var fileStream = File.OpenRead(demoImgPath))
            {
                Google.Apis.Storage.v1.Data.Object uploadResult
                    = await _storageClient.UploadObjectAsync(bucketName, testImgName, contentTypeStr, fileStream);
                await _storageClient.UpdateObjectAsync(uploadResult);
            }

            Exception? delExc = await Record.ExceptionAsync(() => imagesService.DeleteImageFromDrive(testImgName));

            Exception? getAfterDelExc = await Record.ExceptionAsync(() => _storageClient.GetObjectAsync(bucketName, testImgName));

            Assert.Null(delExc);
            Assert.IsType<GoogleApiException>(getAfterDelExc);
            Assert.Equal(HttpStatusCode.NotFound, (getAfterDelExc as GoogleApiException).HttpStatusCode);
        }

        public override void Dispose()
        {
            (imagesService as IDisposable)?.Dispose();
        }
    }
}
