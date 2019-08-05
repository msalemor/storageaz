using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MoveBlob1
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public async static Task Run([BlobTrigger("input-container/{name}", Connection = "")]CloudBlockBlob myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: ? Bytes");
            await CopyBlobAsync(myBlob, log);
        }

        private async static Task CopyBlobAsync(CloudBlockBlob myBlob, ILogger log)
        {
            var key = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;";
            var accountName = "devstoreaccount1";
            var protocol = "https";
            var connectionString = $"DefaultEndpointsProtocol={protocol};AccountName={accountName};AccountKey={key}";
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");

            // Create the destination blob client
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("process-container");
            // Create the container if it doesn't already exist.
            try
            {
                await container.CreateIfNotExistsAsync();
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
            }

            // Get hold of the destination blob
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(myBlob.Name);
            log.LogInformation($"BlockBlob destination name: {blockBlob.Name}");

            log.LogInformation("Starting Copy");
            try
            {
                // Use this method to copy to blob 
                await blockBlob.StartCopyAsync(myBlob);

                // alternatively, uncomment the code below and use this method to copy the blob data
                /*using (var stream = await myBlob.OpenReadAsync())
                {
                  await blockBlob.UploadFromStreamAsync(stream);
                }*/
                await myBlob.DeleteAsync();
                log.LogInformation("Copy completed");

            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                log.LogInformation("Copy failed");
            }
            finally
            {
                log.LogInformation("Operation completed");
            }
        }
    }
}
