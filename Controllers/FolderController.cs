using AzureFuncBe.ContainerManager;
using AzureFuncBe.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFuncBe.Controllers
{
    public class FolderController
    {
        private readonly ILogger<FolderController> _logger;
        private readonly FolderService _folderService;

        public FolderController(ILogger<FolderController> logger, FolderService folderService)
        {
            _logger = logger;
            _folderService = folderService;
        }

        public async Task<IActionResult> GetSingleFolder(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "User/{userId}/Folder{folderId}")]
            HttpRequest req,
            string userId,
            string folderId
        )
        {
            
        }
    }
}