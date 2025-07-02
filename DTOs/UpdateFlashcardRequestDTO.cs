using AzureFuncBe.Models;
using Newtonsoft.Json;

namespace AzureFuncBe.DTOs
{
    public class UpdateFlashcardRequestDTO
    {
        public string? CardFrontText { get; set; }
        public string? CardBackText { get; set; }
        public string? Proficiency { get; set; }
        public List<string>? Tags { get; set; }
        public bool IsFavorite { get; set; }
    }
}
