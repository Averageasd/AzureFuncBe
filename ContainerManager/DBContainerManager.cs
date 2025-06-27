using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace AzureFuncBe.ContainerManager
{
    public class DBContainerManager
    {
        private readonly IConfiguration _configuration;
        private readonly CosmosClient _cosmosClient;
        private readonly string _databaseName;
        private readonly string _flashcardContainer;
        private readonly string _userContainer;

        public DBContainerManager(CosmosClient cosmosClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _cosmosClient = cosmosClient;
            _databaseName = _configuration["CosmosDb:DatabaseName"]!;
            _flashcardContainer = _configuration["FlashcardContainerName"]!;
            _userContainer = _configuration["UserContainerName"]!;
        }

        public Container GetContainer(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentException("Container name cannot be null or empty.", nameof(containerName));
            }
            return _cosmosClient.GetContainer(_databaseName, containerName);
        }

        public string GetFlashcardContainerName()
        {
            return _flashcardContainer;
        }

        public string GetUserContainerName()
        {
            return _userContainer;
        }
    }
}
