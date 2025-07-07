namespace AzureFuncBe.DTOs.FolderDTOs
{
    public class CreateFolderRequestDTO
    {
        public string? Name { get; set; } = "";
        public int IsFavorite { get; set; } = 0;
        public string? FolderDescription { get; set; } = "";
    }
}
