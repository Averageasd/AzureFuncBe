using AzureFuncBe.DTOs.FolderDTOs;
using AzureFuncBe.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Globalization;

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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/{userId}/Folder/{folderId}")]
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "User/{userId}/Folder")]
            HttpRequest req,
            string userId
         )
        {
            try
            {
                string? continuationToken = req.Headers["continuationToken"];
                string? isFavoriteString = req.Query["folderIsFavorite"];
                string? dateSearchMin = req.Query["createdDateSearchMin"];
                string? dateSearchMax = req.Query["createdDateSearchMax"];
                string? orderedProperty = req.Query["orderedProperty"];
                string? sortDirection = req.Query["sortDirection"];
                string? createdByUsernameSearch = req.Query["createdByUsernameSearch"];
                string? folderNameSearch = req.Query["folderNameSearch"];
                PaginatedFoldersSearchDTO paginatedFoldersSearchDTO = new PaginatedFoldersSearchDTO
                {
                    ContinuationToken = continuationToken ?? string.Empty,
                    FolderNameSearch = string.IsNullOrEmpty(folderNameSearch) ? string.Empty : folderNameSearch,
                    CreatedByUsernameSearch = string.IsNullOrEmpty(createdByUsernameSearch) ? string.Empty : createdByUsernameSearch,
                    OrderedProperty = string.IsNullOrEmpty(orderedProperty) ? OrderPropertiesConstants.CreatedAt : orderedProperty,
                    SortDirection = string.IsNullOrEmpty(sortDirection) ? OrderPropertiesConstants.DescOrder : sortDirection
                };
                if (!string.IsNullOrEmpty(dateSearchMin) && DateTimeOffset.TryParse(dateSearchMin, out var createdDateSearchMin))
                {
                    paginatedFoldersSearchDTO.CreatedDateSearchMin = DateTime.ParseExact(
                        dateSearchMin,
                        "yyyy-MM-ddTHH:mm:ssZ",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal);
                }
                if (!string.IsNullOrEmpty(dateSearchMax) && DateTimeOffset.TryParse(dateSearchMax, out var createdDateSearchMax))
                {
                    paginatedFoldersSearchDTO.CreatedDateSearchMax = DateTime.ParseExact(
                        dateSearchMax,
                        "yyyy-MM-ddTHH:mm:ssZ",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal);
                }
                if (!string.IsNullOrEmpty(isFavoriteString) && int.TryParse(isFavoriteString, out var isFavoriteInt))
                {
                    paginatedFoldersSearchDTO.FolderIsFavorite = isFavoriteInt;
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
        [Function("CreateNewFolder")]
        public async Task<IActionResult> CreateFolder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "User/{userId}/Folder")]
            HttpRequest req,
            string userId
        )
        {
            try
            {
                CreateFolderRequestDTO createFolderRequestDTO = await req.ReadFromJsonAsync<CreateFolderRequestDTO>();
                await _folderService.CreateFolderAsync(userId, createFolderRequestDTO!);
                return new CreatedResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        [Function("UpdateFolder")]
        public async Task<IActionResult> UpdateFolder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "User/{userId}/Folder/{folderId}")]
            HttpRequest req,
            string userId,
            string folderId
        )
        {
            try
            {
                FolderUpdateRequestDTO updateFolderRequestDTO = await req.ReadFromJsonAsync<FolderUpdateRequestDTO>();
                await _folderService.UpdateFolderAsync(userId, folderId, updateFolderRequestDTO!);
                return new NoContentResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        [Function("DeleteFolder")]
        public async Task<IActionResult> DeleteFolder(
             [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "User/{userId}/Folder/{folderId}")]
             HttpRequest req,
            string userId,
            string folderId
            )
        {
            try
            {
                await _folderService.DeleteFolderAsync(userId, folderId);
                return new NoContentResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}