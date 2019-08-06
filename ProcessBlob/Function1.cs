using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ProcessBlob
{
    public static class Function1
    {
        // CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
        static CloudStorageAccount storageAccount = CloudStorageAccount.Parse("UseDevelopmentStorage=true");

        // Create the destination blob client
        static CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

        [FunctionName("Function1")]
        public static async Task Run([BlobTrigger("process-container/{name}", Connection = "")]CloudBlockBlob myBlob, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{myBlob.Properties.Length} \n Size: ? Bytes");

            // Once the blob is posted to the process-container process it
            var json = await ProcessAsync(myBlob, log);
            log.LogInformation("Saving the json payload after processing");

            // Save the JSON that was deserialized to a blob on the json-container
            await SaveBlob(json, log);

            // Move the blob from the original location to the archive-container
            log.LogInformation("Moving the processed blob to archive");
            await MoveBlob(myBlob, log);

        }

        private async static Task<string> ProcessAsync(CloudBlockBlob blob, ILogger log)
        {
            // Read the blob's bytes
            var buffer = new byte[blob.Properties.Length];
            await blob.DownloadToByteArrayAsync(buffer, 0);

            // Convert the bytes into a Base64 string
            var base64Encoded = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
            byte[] data = Convert.FromBase64String(base64Encoded);

            // Convert the Base64 to the JSON string
            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        private async static Task SaveBlob(string json, ILogger log)
        {

            // Get a reference to the json-container
            CloudBlobContainer container = blobClient.GetContainerReference("json-container");

            // Create the container if it doesn't already exist.
            try
            {
                await container.CreateIfNotExistsAsync();
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
            }
            
            // Get a blob reference for the new blob
            var blobName = $"{Environment.TickCount.ToString()}.json";
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

            // Get the bytes for the JSON
            var buffer = Encoding.ASCII.GetBytes(json);
            var count = json.Length;
            
            // Save the bytes to blob reference
            await blockBlob.UploadFromByteArrayAsync(buffer, 0, count);
            log.LogInformation($"Blob {blobName} saved to container json-container");
        }

        private async static Task MoveBlob(CloudBlockBlob myBlob, ILogger log)
        {
            // Get a refence to the archive-container
            CloudBlobContainer container = blobClient.GetContainerReference("archive-container");

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
                // Copy the original blob to the new location
                await blockBlob.StartCopyAsync(myBlob);

                // alternatively, uncomment the code below and use this method to copy the blob data
                /*using (var stream = await myBlob.OpenReadAsync())
                {
                  await blockBlob.UploadFromStreamAsync(stream);
                }*/
                log.LogInformation("Starting Delete");

                // Delete the original blob
                await myBlob.DeleteAsync();
                log.LogInformation("Move completed");

            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
                log.LogInformation("Move failed");
            }
            finally
            {
                log.LogInformation("Operation completed");
            }
        }
    }
}
