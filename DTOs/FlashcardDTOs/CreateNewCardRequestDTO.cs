using AzureFuncBe.Models;

namespace AzureFuncBe.DTOs.FlashcardDTOs
{
    public class CreateNewCardRequestDTO
    {
        public string? CardFrontText { get; set; } = "";
        public string? CardBackText { get; set; } = "";
        public List<string>? Tags { get; set; } = new List<string>();
        public int IsFavorite { get; set; } = 0;

        public long CardCount { get; set; }
    }
}
