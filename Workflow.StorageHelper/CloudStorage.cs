using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace Workflow.StorageHelper
{
    public class CloudStorage
    {
        public static CloudBlobClient GetClient(string key)
        {
            if (key is null)
            {
                key = "UseDevelopmentStorage = true";
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(key);
            
            return storageAccount.CreateCloudBlobClient();
        }

        public static CloudBlobContainer GetContainer(CloudBlobClient client, string containerName)
        {
            return client.GetContainerReference(containerName);
        }

        public static async Task CreateIfNotExistsAsync(CloudBlobContainer container, string containerName)
        {
            try
            {
                await container.CreateIfNotExistsAsync();
            }
            catch (Exception)
            {
                //log.LogError(e.Message);
            }
        }

    }
}
