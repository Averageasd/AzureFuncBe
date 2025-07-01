namespace AzureFuncBe.DTOs
{
    public class FolderUpdateRequestDTO
    {
        public string? Name { get; set; } = string.Empty;
        public string? FolderDescription { get; set; } = string.Empty;
        public bool IsFavorite { get; set; } = false;

    }
}
