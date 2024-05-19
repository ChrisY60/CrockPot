using Azure.Storage.Blobs;
using CrockPot.Services.IServices;
using Azure;
using Azure.Identity;

namespace CrockPot.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobService(IConfiguration configuration)
        {
            var blobServiceUri = new Uri(configuration["BlobStorage:Uri"]);
            DefaultAzureCredential blobCredential = new DefaultAzureCredential();
            _blobServiceClient = new BlobServiceClient(blobServiceUri, blobCredential);
            _containerName = configuration["BlobStorage:ContainerName"];
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            try
            {
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                 
                if (await containerClient.ExistsAsync())
                {
                    string blobName = Guid.NewGuid().ToString(); 
                    BlobClient blockBlob = containerClient.GetBlobClient(blobName);

                    using (Stream? stream = file.OpenReadStream())
                    {
                        await blockBlob.UploadAsync(stream, true);
                        return blockBlob.Uri.ToString();
                    }
                }
                else
                {
                    throw new InvalidOperationException("Container not found.");
                }
            }
            catch (RequestFailedException ex)
            {
                throw new ApplicationException("Blob storage request failed" + ex.Message);
            }
        }
    }
}
