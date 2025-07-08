using AzureFuncBe.ContainerManager;
using AzureFuncBe.Models;
using AzureFuncBe.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AzureFuncBe.Controllers
{
    public class LocalController
    {
        private readonly DBContainerManager _dbContainerManager;
        private readonly IConfiguration _configuration;
        private string _uploadMulitiFolderPath;
        private GenerateNewDateUtil _generateNewDateUtil;
        public LocalController(DBContainerManager dBContainerManager, IConfiguration configuration, GenerateNewDateUtil generateNewDateUtil)
        {
            _dbContainerManager = dBContainerManager;
            _configuration = configuration;
            _uploadMulitiFolderPath = _configuration["TestUploadMultipleFoldersPath"]!;
            _generateNewDateUtil = generateNewDateUtil;
        }
        [Function("UploadMultiFilesTest")]
        public async Task<IActionResult> LocalTestUploadMultipleFiles(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "LocalController/TestPostMultiFiles/User/{userId}")]
            HttpRequest req,
            string userId
        )
        {
            try
            {
                string jsonString = File.ReadAllText(_uploadMulitiFolderPath);
                List<FolderModel> folders = JsonConvert.DeserializeObject<List<FolderModel>>(jsonString);
                Container container = _dbContainerManager.GetContainer(_dbContainerManager.GetFolderContainerName());
                List<Task> tasks = new List<Task>();
                foreach (FolderModel item in folders!)
                {
                    item.Id = Guid.NewGuid().ToString();
                    item.UserId = userId;
                    item.CreatedDate = GenerateNewDateUtil.GenerateNewDate(DateTimeOffset.Now);
                    tasks.Add(container.CreateItemAsync(item, new PartitionKey(item.UserId))
                        .ContinueWith(itemResponse =>
                        {
                            if (!itemResponse.IsCompletedSuccessfully)
                            {
                                AggregateException innerExceptions = itemResponse.Exception.Flatten();
                                if (innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
                                {
                                    Console.WriteLine($"Received {cosmosException.StatusCode} ({cosmosException.Message}).");
                                }
                                else
                                {
                                    Console.WriteLine($"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
                                }
                            }
                            else
                            {
                                Console.WriteLine(itemResponse.Result);
                            }
                        }));
                }

                await Task.WhenAll(tasks);
                return new OkResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}
