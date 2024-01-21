using Azure.Storage.Blobs;
using CrockPot.Services.IServices;
using Azure;
using Azure.Identity;

namespace CrockPot.Services
{
    public class BlobService : IBlobService
    {
        public async Task<string> UploadImageAsync(IFormFile file)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(
                    new Uri("https://crockpotblob2005.blob.core.windows.net"),
                    new DefaultAzureCredential());

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");
                 
                if (await containerClient.ExistsAsync())
                {
                    var blobName = Guid.NewGuid().ToString(); 
                    var blockBlob = containerClient.GetBlobClient(blobName);

                    using (var stream = file.OpenReadStream())
                    {
                        await blockBlob.UploadAsync(stream, true);
                        return blockBlob.Uri.ToString();
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Container not found.");
                }
            }
            catch (RequestFailedException ex)
            {
                throw new ApplicationException($"Blob storage request failed: {ex.Message}", ex);
            }
        }

        public async Task<string> GetImageUrlAsync(string blobName)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(
                    new Uri("https://crockpotblob2005.blob.core.windows.net"),
                    new DefaultAzureCredential());

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");

                var blockBlob = containerClient.GetBlobClient(blobName);

                if (await blockBlob.ExistsAsync())
                {
                    return blockBlob.Uri.ToString();
                }

                return null;
            }
            catch (RequestFailedException ex)
            {
                throw new ApplicationException($"Blob storage request failed: {ex.Message}", ex);
            }
        }

    }
}
