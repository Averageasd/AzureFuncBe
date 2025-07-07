namespace AzureFuncBe.DTOs.FlashcardDTOs
{
    public class PaginatedFlashcardSearchDTO
    {
        public string? ContinuationToken { get; set; }
        public string? CardFrontBackTextSearch { get; set; } = string.Empty;
    }
}
