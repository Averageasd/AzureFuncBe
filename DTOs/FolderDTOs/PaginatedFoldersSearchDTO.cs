namespace AzureFuncBe.DTOs.FolderDTOs
{
    public class OrderPropertiesConstants
    {
        public const string CreatedAt = "createdAt";
        public const string FolderName = "folderName";
        public const string CardCount = "cardCount";
        public const string CreatedBy = "createdBy";
        public const string AscOrder = "ASC";
        public const string DescOrder = "DESC";

    }
    public class PaginatedFoldersSearchDTO
    {
        public string ContinuationToken { get; set; } = string.Empty;
        public string FolderNameSearch { get; set; } = string.Empty;
        public int FolderIsFavorite { get; set; } = -1;
        public string CreatedByUsernameSearch { get; set; } = string.Empty;

        public DateTimeOffset CreatedDateSearchMin { get; set; } = DateTimeOffset.MinValue;
        public DateTimeOffset CreatedDateSearchMax { get; set; } = DateTimeOffset.MaxValue;

        public string OrderedProperty { get; set; } = OrderPropertiesConstants.CreatedAt;
        public string SortDirection { get; set; } = OrderPropertiesConstants.DescOrder;

    }
}
