namespace AzureFuncBe.DTOs.FolderDTOs
{
    public class FolderUpdateRequestDTO
    {
        public string? Name { get; set; } = string.Empty;
        public string? FolderDescription { get; set; } = string.Empty;
        public int IsFavorite { get; set; } = 0;

    }
}
