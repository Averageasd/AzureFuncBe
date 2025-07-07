namespace AzureFuncBe.DTOs.FlashcardDTOs
{
    public class PaginatedFlashcardDTO
    {
        public List<SingleFlashcardResponseDTO> ListResponses { get; set; } = new List<SingleFlashcardResponseDTO>();
        public string? ContinuationToken { get; set; }
    }
}
