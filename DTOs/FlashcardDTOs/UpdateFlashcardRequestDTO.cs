using AzureFuncBe.Models;
using Newtonsoft.Json;

namespace AzureFuncBe.DTOs.FlashcardDTOs
{
    public class UpdateFlashcardRequestDTO
    {
        public string? CardFrontText { get; set; }
        public string? CardBackText { get; set; }
        public string? Proficiency { get; set; }
        public List<string>? Tags { get; set; }
        public int IsFavorite { get; set; }
    }
}
