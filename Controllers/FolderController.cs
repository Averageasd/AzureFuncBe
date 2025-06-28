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

        [Function("GetSingleFolder")]
        public async Task<IActionResult> GetSingleFolder(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "User/{userId}/Folder/{folderId}")]
            HttpRequest req,
            string userId,
            string folderId
        )
        {
            try
            {
                var singleFolder = await _folderService.GetSingleFolderAsync(userId, folderId);
                if (singleFolder == null)
                {
                    throw new Exception();
                }
                return new OkObjectResult(singleFolder);
            }
            catch (Exception) { 
                return new StatusCodeResult(500);
            }
        }
    }
}