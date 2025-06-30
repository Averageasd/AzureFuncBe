using AzureFuncBe.DTOs;
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
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        [Function("GetPaginatedFolders")]
        public async Task<IActionResult> GetPaginatedFolders(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "User/{userId}/Folder")]
            HttpRequest req,
            string userId
         )
        {
            try
            {
                string? continuationToken = req.Headers["continuationToken"]!;
                string isFavoriteString = req.Query["folderIsFavorite"]!;
                string? dateSearchMin = req.Query["createdDateSearchMin"]!;
                string? dateSearchMax = req.Query["createdDateSearchMax"]!;
                PaginatedFoldersSearchDTO paginatedFoldersSearchDTO = new PaginatedFoldersSearchDTO
                {
                    ContinuationToken = continuationToken ?? string.Empty,
                    FolderNameSearch = req.Query["folderNameSearch"]!,
                    CreatedByUsernameSearch = req.Query["createdByUsernameSearch"]!
                };  
                if (!string.IsNullOrEmpty(dateSearchMin) && DateOnly.TryParse(dateSearchMin, out var createdDateSearchMin))
                {
                    paginatedFoldersSearchDTO.CreatedDateSearchMin = createdDateSearchMin;
                }
                if (!string.IsNullOrEmpty(dateSearchMax) && DateOnly.TryParse(dateSearchMax, out var createdDateSearchMax))
                {
                    paginatedFoldersSearchDTO.CreatedDateSearchMax = createdDateSearchMax;
                }
                if (!string.IsNullOrEmpty(isFavoriteString) && isFavoriteString.Equals("true")) {
                    paginatedFoldersSearchDTO.FolderIsFavorite = true;
                }
                var paginatedFolders = await _folderService.GetFoldersAsync(userId, paginatedFoldersSearchDTO);
                if (paginatedFolders == null)
                {
                    throw new Exception();
                }
                req.HttpContext.Response.Headers.Append("x-my-token", paginatedFolders.ContinuationToken);
                return new OkObjectResult(paginatedFolders);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}