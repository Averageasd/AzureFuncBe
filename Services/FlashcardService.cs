using AzureFuncBe.ContainerManager;
using AzureFuncBe.DTOs;
using AzureFuncBe.Models;
using Microsoft.Azure.Cosmos;

namespace AzureFuncBe.Services
{
    // now implement create card service
    // we need to create flashcard model
    public class FlashcardService
    {
        private DBContainerManager _dBContainerManager;

        public FlashcardService
        (
            DBContainerManager dBContainerManager
        )
        {
            _dBContainerManager = dBContainerManager;

        }

        // step 1: take in userId, folderId and create CreateNewCardRequestDTO
        // step 2: create new card model with values supplied by dto
        // step 3: save it in cosmodb 
        // step 4: call this method from controller
        public async Task CreateNewFlashcardAsync(string userId, string folderId, CreateNewCardRequestDTO createNewCardRequestDTO)
        {
            var container = _dBContainerManager.GetContainer(_dBContainerManager.GetFlashcardContainerName());

            var card = new FlashcardModel()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                FolderId = folderId,
                CardFrontText = createNewCardRequestDTO.CardFrontText,
                CardBackText = createNewCardRequestDTO.CardBackText,
                IsFavorite = createNewCardRequestDTO.IsFavorite,
                Tags = createNewCardRequestDTO.Tags,
                StudyTimes = 0,
                Proficiency = ProficiencyConstants.NOT_LEARN,
                CreatedDate = DateOnly.FromDateTime(DateTime.Today)
            };

            try
            {
                var response = await container.CreateItemAsync(card, new Microsoft.Azure.Cosmos.PartitionKey(card.UserId));
                if (response.StatusCode != System.Net.HttpStatusCode.Created)
                {
                    throw new Exception("Failed to create card");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating folder: {ex.Message}");
                throw;
            }
        }

        // step 1: get database reference for flashcard
        // step 2: get single item from cosmodb using flashcard id
        // step 3: check from controller if folder exists, if not, throw exception
        // step 4: if exists, call this method
        public async Task<SingleFlashcardResponseDTO> GetSingleFlashcardAsync(string flashcardId, string userId)
        {
            var container = _dBContainerManager.GetContainer(_dBContainerManager.GetFlashcardContainerName());
            try
            {
                ItemResponse<SingleFlashcardResponseDTO> response = await container.ReadItemAsync<SingleFlashcardResponseDTO>(flashcardId, new Microsoft.Azure.Cosmos.PartitionKey(userId));
                SingleFlashcardResponseDTO item = response.Resource;
                return item;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateFlashcardAsync(string flashcardId, string userId, UpdateFlashcardRequestDTO updateFlashcardRequestDTO)
        {
            var container = _dBContainerManager.GetContainer(_dBContainerManager.GetFlashcardContainerName());
            try
            {
                var patchOperations = new PatchOperation[]
                {
                    PatchOperation.Replace("/cardFront", updateFlashcardRequestDTO.CardFrontText),
                    PatchOperation.Replace("/cardBack", updateFlashcardRequestDTO.CardBackText),
                    PatchOperation.Replace("/isFavorite", updateFlashcardRequestDTO.IsFavorite),
                    PatchOperation.Replace("/proficiency", updateFlashcardRequestDTO.Proficiency)
                };
                TransactionalBatch batch = container.CreateTransactionalBatch(new PartitionKey(userId));
                batch.PatchItem(flashcardId, patchOperations);
                TransactionalBatchResponse response = await batch.ExecuteAsync();
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
                throw new Exception();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteFlashcardAsync(string flashcardId, string userId)
        {
            var container = _dBContainerManager.GetContainer(_dBContainerManager.GetFlashcardContainerName());
            try
            {
                var response = await container.DeleteItemAsync<FolderModel>(flashcardId, new PartitionKey(userId));
                if (response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    throw new Exception("Failed to delete flashcard");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
