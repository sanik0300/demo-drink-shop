using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Configuration;

namespace DrinkShopTests
{
    public sealed class GoogleStorageApiTest : StorageRelatedTest
    {
        [Fact]
        public void AddFileTest()
        {
            Google.Apis.Storage.v1.Data.Object obj=null;
            using (var fileStream = File.OpenRead(demoImgPath))
            {
                Exception? exc = Record.Exception(() => { 
                    obj = _storageClient.UploadObject(bucketName, testImgName, contentTypeStr, fileStream);
                    _storageClient.UpdateObject(obj);
                });
                _storageClient.DeleteObject(bucketName, testImgName);

                Assert.Null(exc);
            }
        }

        [Fact]
        public void ReplaceFileTest()
        {
            using (var fileStream = File.OpenRead(demoImgPath))
            {
                Exception? exc = Record.Exception(() => {
                    _storageClient.UploadObject(bucketName, testImgName, contentTypeStr, fileStream);
                });
                Assert.Null(exc);
            }

            byte[] img1Bytes, img2Bytes;

            using (var memStream1 = new MemoryStream())
            {
                _storageClient.DownloadObject(bucketName, testImgName, memStream1);
                img1Bytes = memStream1.ToArray();
            }

            using (var fileStream = File.OpenRead(demoImg2Path))
            {
                Exception? exc = Record.Exception(() => {
                    _storageClient.UploadObject(bucketName, testImgName, contentTypeStr, fileStream);
                });
                Assert.Null(exc);
            }

            using (var memStream2 = new MemoryStream())
            {
                _storageClient.DownloadObject(bucketName, testImgName, memStream2);
                img2Bytes = memStream2.ToArray();
            }

            _storageClient.DeleteObject(bucketName, testImgName);

            Assert.False(img2Bytes.SequenceEqual(img1Bytes));
        }


        [Fact]
        public void DeleteFileTest()
        {
            using (var fileStream = File.OpenRead(demoImgPath))
            {
                _storageClient.UploadObject(bucketName, testImgName, contentTypeStr, fileStream);
            }

            Exception? exc = Record.Exception(() => {
                _storageClient.DeleteObject(bucketName, testImgName);
            });
            Assert.Null(exc);
        }


        public override void Dispose()
        {
            _storageClient.Dispose();
        }
    }
}