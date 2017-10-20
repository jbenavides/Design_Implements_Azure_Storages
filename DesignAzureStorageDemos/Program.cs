using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DesignAzureStorageDemos
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             Install-Package WindowsAzure.Storage
             Install-Package Microsoft.WindowsAzure.ConfigurationManager
             */

            // creating a blob storage
            //CreateBlobContainer();

            // upload blobk to container
            //UploadBlobDataToContainer();

            // read blobs from container
            //ReadingBlobsFromContainer();

            //download blob from storage
            //DownloadBlobFromContainer();

            // Async blob copy
            //AsyncBlobCopy();

            // blob Hierarchies
            // BlobHierarchies();

            //Set metadata to a container
            //SetMetadataOnContainer();

            // read metadata from a container
            ReadMetaDataOnContainer();

            Console.ReadLine();




        }

        private static void CreateBlobContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnection"));

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("images");

            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
        }

        private static void UploadBlobDataToContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnection"));

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("images");

            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            var blockBlob = container.GetBlockBlobReference("demoImageSMC.jpg");

            using (var fileStream = File.OpenRead(@"C:\jbenavides\img2.jpg"))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }

        private static void ReadingBlobsFromContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnection"));

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("images");

            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            var blobs = container.ListBlobs();

            foreach (var blob in blobs)
            {
                Console.WriteLine(blob.Uri);
            }

            Console.ReadLine();
        }

        private static void DownloadBlobFromContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnection"));

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("images");

            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            var blockBlob = container.GetBlockBlobReference("demoImageSMC.jpg");

            using (var fileStream = File.OpenWrite(@"C:\jbenavides\demoImageSMC.jpg"))
            {
                blockBlob.DownloadToStream(fileStream);
            }
        }

        private static void AsyncBlobCopy()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnection"));

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("images");

            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            // in real escenario we can copy from one container to another
            var blockBlob = container.GetBlockBlobReference("demoImageSMC.jpg");
            var blockBlobCopy = container.GetBlockBlobReference("demoImageSMC_COPY.jpg");
            
            var cb = new AsyncCallback(x=> Console.WriteLine("blob copy completed!"));

            blockBlobCopy.BeginStartCopy(blockBlob.Uri, cb, null);
        }

        private static void BlobHierarchies()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnection"));

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("images");

            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);
            
            // in the blob container we can not create folders but we can use "prefix" in the blob name's
            var blockBlob = container.GetBlockBlobReference("jpg-images/img4.jpg");

            using (var fileStream = File.OpenRead(@"C:\jbenavides\img2.jpg"))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }

        private static void SetMetadataOnContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnection"));

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("images");

            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            SetMetaData(container);
        }

        private static void ReadMetaDataOnContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnection"));

            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("images");

            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            GetMetaData(container);
        }

        private static void SetMetaData(CloudBlobContainer container)
        {
            container.Metadata.Clear();
            container.Metadata.Add("Owner", "Jose");
            container.Metadata["Updated"] = DateTime.Now.ToString();
            container.SetMetadata();
        }

        private static void GetMetaData(CloudBlobContainer container)
        {
            container.FetchAttributes();
            Console.WriteLine("Container MetaData: \n");
            foreach (var item in container.Metadata)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
        }
    }
}
