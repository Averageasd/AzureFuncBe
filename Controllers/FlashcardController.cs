using AzureFuncBe.DTOs;
using AzureFuncBe.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Identity.Client;

namespace AzureFuncBe.Controllers
{
    // step 1: inject needed services
    // step 2: create controller headers
    // step 3: create flashcard create card service
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
            HttpRequest req,
            string userId,
            string folderId,
            string flashcardId
        )
        {
            return new OkResult();
        }

        [Function("GetPaginatedFlashcards")]
        public async Task<IActionResult> GetPaginatedFlashCards(
         [HttpTrigger(AuthorizationLevel.Function, "get", Route = "User/{userId}/Folder/{folderId}/Flashcard")]
             HttpRequest req,
             string userId,
             string folderId
        )
        {
            return new OkResult();
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
            return new NoContentResult();
        }

        [Function("DeleteFlashCard")]
        public async Task<IActionResult> DeleteFlashCard(
         [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "User/{userId}/Folder/{folderId}/Flashcard/{flashcardId}")]
             HttpRequest req,
             string userId,
             string folderId,
             string flashcardId
        )
        {
            return new NoContentResult();
        }




    }
}
