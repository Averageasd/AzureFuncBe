using Azure.Storage.Blobs;

namespace AzureFuncBe.ContainerManager
{
    public class BlobContainerManager
    {
        private readonly BlobServiceClient _blobServiceClient;
        public BlobContainerManager(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        public async Task<BlobContainerClient> GetContainerWithName(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            return containerClient;
        }
    }
}
