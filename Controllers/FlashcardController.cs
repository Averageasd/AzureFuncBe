using AzureFuncBe.DTOs.FlashcardDTOs;
using AzureFuncBe.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureFuncBe.Controllers
{
    public class FlashcardController
    {
        private readonly FlashcardService _flashcardService;
        private readonly FolderService _folderService;

        public FlashcardController(FlashcardService flashcardService, FolderService folderService)
        {
            _flashcardService = flashcardService;
            _folderService = folderService;
        }

        [Function("CreateFlashCard")]

        public async Task<IActionResult> CreateFlashCard(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "User/{userId}/Folder/{folderId}/Flashcard")]
             HttpRequest req,
             string userId,
             string folderId
        )
        {
            try
            {
                var createNewCardDTO = await req.ReadFromJsonAsync<CreateNewCardRequestDTO>();
                var existingFolder = await _folderService.GetSingleFolderAsync(userId, folderId);
                // check if current folder exists before creating a new card in it
                if (existingFolder == null)
                {
                    throw new Exception();
                }
                await _flashcardService.CreateNewFlashcardAsync(userId, folderId, createNewCardDTO);
                await _folderService.IncrementFolderCardCountAsync(userId, folderId);
                return new CreatedResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }


        }

        [Function("GetSingleFlashcard")]
        public async Task<IActionResult> GetFlashCardById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "User/{userId}/Folder/{folderId}/Flashcard/{flashcardId}")]
            string userId,
            string folderId,
            string flashcardId
        )
        {

            try
            {
                var existingFolder = await _folderService.GetSingleFolderAsync(userId, folderId);
                // check if current folder exists before getting a card
                if (existingFolder == null)
                {
                    throw new Exception();
                }
                var singleCard = await _flashcardService.GetSingleFlashcardAsync(flashcardId, userId);
                return new OkObjectResult(singleCard);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        [Function("UpdateFlashCard")]
        public async Task<IActionResult> UpdateFlashCard(
         [HttpTrigger(AuthorizationLevel.Function, "put", Route = "User/{userId}/Folder/{folderId}/Flashcard/{flashcardId}")]
             HttpRequest req,
             string userId,
             string folderId,
             string flashcardId
        )
        {
            try
            {
                var existingFolder = await _folderService.GetSingleFolderAsync(userId, folderId);
                // check if current folder exists before updating a card
                if (existingFolder == null)
                {
                    throw new Exception();
                }

                var updateFlashcardDTO = await req.ReadFromJsonAsync<UpdateFlashcardRequestDTO>();
                await _flashcardService.UpdateFlashcardAsync(flashcardId, userId, updateFlashcardDTO);
                return new NoContentResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        [Function("DeleteFlashCard")]
        public async Task<IActionResult> DeleteFlashCard(
         [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "User/{userId}/Folder/{folderId}/Flashcard/{flashcardId}")]
             string userId,
             string folderId,
             string flashcardId
        )
        {
            try
            {
                var existingFolder = await _folderService.GetSingleFolderAsync(userId, folderId);
                // check if current folder exists before deleting a card
                if (existingFolder == null)
                {
                    throw new Exception();
                }

                await _flashcardService.DeleteFlashcardAsync(flashcardId, userId);
                return new NoContentResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        [Function("GetPaginatedFlashcards")]
        public async Task<IActionResult> GetPaginatedFlashcards(
         [HttpTrigger(AuthorizationLevel.Function, "get", Route = "User/{userId}/Folder/{folderId}/Flashcard")]
             HttpRequest req,
             string userId,
             string folderId
        )
        {
            try
            {
                var existingFolder = await _folderService.GetSingleFolderAsync(userId, folderId);
                // check if current folder exists before deleting a card
                if (existingFolder == null)
                {
                    throw new Exception();
                }

                string? continuationToken = req.Headers["continuationToken"];
                string? cardFrontBackTextSearch = req.Query["cardFrontBackTextSearch"];
                string? tagSearch = req.Query["tagSearch"];
                string? isFavorite = req.Query["isFavorite"];
                string? proficiency = req.Query["proficiency"];
                string? orderProperty = req.Query["orderProperty"];
                string? sortDirection = req.Query["sortDirection"];
                string? createdDateSearchMin = req.Query["createdDateSearchMin"];
                string? createdDateSearchMax = req.Query["createdDateSearchMax"];

                PaginatedFlashcardSearchDTO paginatedFlashcardSearchDTO = new PaginatedFlashcardSearchDTO()
                {
                    ContinuationToken = continuationToken ?? null,
                    CardFrontBackTextSearch = cardFrontBackTextSearch ?? string.Empty,
                    TagSearch = cardFrontBackTextSearch ?? string.Empty,
                    Proficiency = proficiency ?? string.Empty,
                    OrderProperty = orderProperty ?? FlashCardSearchOrderProperties.CreatedAt,
                    SortDirection = sortDirection ?? FlashCardSearchOrderProperties.DescOrder
                };
                if (!string.IsNullOrEmpty(isFavorite) && int.TryParse(isFavorite, out var parsedIsFavorite))
                {
                    if (parsedIsFavorite < -1 || parsedIsFavorite > 2)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        paginatedFlashcardSearchDTO.IsFavorite = parsedIsFavorite;
                    }
                }

                if (!string.IsNullOrEmpty(createdDateSearchMin))
                {
                    paginatedFlashcardSearchDTO.CreatedDateSearchMin = createdDateSearchMin + "+00:00";
                }

                if (!string.IsNullOrEmpty(createdDateSearchMax))
                {
                    paginatedFlashcardSearchDTO.CreatedDateSearchMax = createdDateSearchMax + "+00:00";
                }

                var paginatedFlashcards = await _flashcardService.GetFlashcardsAsync(folderId, paginatedFlashcardSearchDTO);
                req.HttpContext.Response.Headers.Append("x-my-token", paginatedFlashcards.ContinuationToken);

                return new OkObjectResult(paginatedFlashcards);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}
