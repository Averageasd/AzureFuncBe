namespace AzureFuncBe.DTOs
{
    public class CreateFolderRequestDTO
    {
        public string? Name { get; set; } = "";
        public bool IsFavorite { get; set; } = false;
        public string? FolderDescription { get; set; } = "";
    }
}
