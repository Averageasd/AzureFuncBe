﻿namespace AzureFuncBe.DTOs
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
        public bool FolderIsFavorite { get; set; } = false;
        public string CreatedByUsernameSearch { get; set; } = string.Empty;

        public DateOnly CreatedDateSearchMin { get; set; } = DateOnly.MinValue;
        public DateOnly CreatedDateSearchMax { get; set; } = DateOnly.MinValue;

        public string OrderedProperty { get; set; } = OrderPropertiesConstants.CreatedAt;
        public string SortDirection { get; set; } = OrderPropertiesConstants.DescOrder;   

    }
}
