namespace AzureFuncBe.Models
{
    public class FolderModel
    {
        public required string Id { get; set; }
        public string? Name { get; set; }
        public int CardCount { get; set; } = 0;
        public required bool IsFavorite { get; set; }
    }
}
