namespace AzureFuncBe.DTOs
{
    public class PaginatedFoldersSearchDTO
    {
        public string ContinuationToken { get; set; } = string.Empty;  
        public string FolderNameSearch { get; set; } = string.Empty;
        public bool FolderIsFavorite { get; set; } = false;
        public string CreatedByUsernameSearch { get; set; } = string.Empty;

        public DateOnly CreatedDateSearchMin { get; set; } = DateOnly.MinValue;
        public DateOnly CreatedDateSearchMax { get; set; } = DateOnly.MinValue;
    }
}
