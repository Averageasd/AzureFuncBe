namespace AzureFuncBe.DTOs.FolderDTOs
{
    public class PaginatedFoldersDTO
    {
        public List<SingleFolderResponseDTO> ListResponses { get; set; } = new List<SingleFolderResponseDTO>();
        public string? ContinuationToken { get; set; }
    }
}
