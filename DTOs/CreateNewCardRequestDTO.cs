using AzureFuncBe.Models;

namespace AzureFuncBe.DTOs
{
    public class CreateNewCardRequestDTO
    {
        public string? CardFrontText { get; set; } = "";
        public string? CardBackText { get; set; } = "";
        public List<string>? Tags { get; set; } = new List<string>();
        public bool IsFavorite { get; set; } = false;

        public long CardCount { get; set; }
    }
}
