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
                var response = await container.CreateItemAsync(card, new PartitionKey(card.UserId));
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

        
    }
}
