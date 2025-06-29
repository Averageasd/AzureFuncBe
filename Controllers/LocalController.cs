using AzureFuncBe.ContainerManager;
using AzureFuncBe.Models;
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
        public LocalController(DBContainerManager dBContainerManager, IConfiguration configuration)
        {
            _dbContainerManager = dBContainerManager;
            _configuration = configuration;
            _uploadMulitiFolderPath = _configuration["TestUploadMultipleFoldersPath"]!;
        }
        [Function("UploadMultiFilesTest")]
        public async Task<IActionResult> LocalTestUploadMultipleFiles(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "LocalController/TestPostMultiFiles")]
            HttpRequest req,
        string userId,
            string folderId
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
