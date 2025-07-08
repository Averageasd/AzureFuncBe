using AzureFuncBe.Utils;

namespace AzureFuncBe.DTOs.FlashcardDTOs
{
    public class FlashCardSearchOrderProperties
    {
        public const string CreatedAt = "createdAt";
        public const string StudyTimes = "studyTimes";
        public const string AscOrder = "ASC";
        public const string DescOrder = "DESC";
    }
    public class PaginatedFlashcardSearchDTO
    {
        public string? ContinuationToken { get; set; }
        public string? CardFrontBackTextSearch { get; set; } = string.Empty;
        public string? TagSearch { get; set; } = string.Empty;

        public int IsFavorite { get; set; } = -1;

        public string? Proficiency { get; set; } = string.Empty;

        public string? OrderProperty { get; set; } = FlashCardSearchOrderProperties.CreatedAt;

        public string? SortDirection { get; set; } = FlashCardSearchOrderProperties.DescOrder;

        public string CreatedDateSearchMin { get; set; } = GenerateNewDateUtil.GenerateNewDate(DateTimeOffset.MinValue);
        public string CreatedDateSearchMax { get; set; } = GenerateNewDateUtil.GenerateNewDate(DateTimeOffset.MaxValue);
    }
}
