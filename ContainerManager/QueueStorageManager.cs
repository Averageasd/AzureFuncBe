using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;

namespace AzureFuncBe.ContainerManager
{
    public class QueueStorageManager
    {
        private readonly IConfiguration _configuration;

        public QueueStorageManager(IConfiguration configuration)
        {
            _configuration = configuration;
            
        }

        public async Task<QueueClient> GetQueueWithName(string queueName)
        {
            var connectionString = _configuration["AzureQueueStorageConnection"]!;
            QueueClient queueClient = new QueueClient(connectionString, queueName);
            await queueClient.CreateIfNotExistsAsync();
            return queueClient;
        }
    }
}
