using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFuncBe.QueueJob
{
    public class QueueTaskHandler
    {
        private readonly ILogger _logger;
        public QueueTaskHandler(ILogger<QueueTaskHandler> logger)
        {
            _logger = logger;
        }

        //[Function("QueueTaskHandler")]
        //public void TaskHandler(
        //    [QueueTrigger("bulk-insert-fc-queue", Connection = "AzureQueueStorageConnection")] string queueItem)
        //{
        //    try
        //    {
        //        _logger.LogInformation($"Queue message received: {queueItem}");
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e.Message);
        //    }
            
        //}
    }
}
