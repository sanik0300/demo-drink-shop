using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrinkShopTests
{
    public abstract class StorageRelatedTest : IDisposable
    {
        private const string testConfigPath = "tests-config.txt",
                             configFileKey = "FirebaseKeyFileName", configBucketKey = "BucketName";

        protected const string demoImgPath = "images\\test1.jpg", demoImg2Path = "images\\test2.jpg",
                               testImgName = "test", contentTypeStr = "image/jpg";

        protected readonly string bucketName, firebaseAuthPath;
        protected readonly StorageClient _storageClient;

        protected StorageRelatedTest()
        {
            string[] configLines = File.ReadAllLines(testConfigPath);
            var configEntries = configLines.Select(line =>
            {
                string[] kv = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
                return new KeyValuePair<string, string>(kv[0], kv[1]);
            }).ToDictionary();

            bucketName = configEntries[configBucketKey];
            firebaseAuthPath = configEntries[configFileKey];

            GoogleCredential serviceAccCredential;

            using (FileStream fs = new FileStream(firebaseAuthPath, FileMode.Open, FileAccess.Read))
            {
                serviceAccCredential = GoogleCredential.FromStream(fs);
            }

            _storageClient = StorageClient.Create(serviceAccCredential);
        }

        public abstract void Dispose();
    }
}
