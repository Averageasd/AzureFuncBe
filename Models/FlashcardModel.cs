using Newtonsoft.Json;

namespace AzureFuncBe.Models
{
    public class ProficiencyConstants
    {
        public const string NOT_LEARN = "NOT LEARNT";
        public const string NOT_COMFORTABLE = "DIFFICULT";
        public const string HIGH_MASTERY = "HIGH MASTERY";
        public const string NEED_MORE_TIME = "MORE TIME";
    }

    public class FlashcardModel
    {
        [JsonProperty("id")]
        public required string Id { get; set; }
        [JsonProperty("userId")]
        public required string UserId { get; set; }
        [JsonProperty("folderId")]
        public required string FolderId { get; set; }
        [JsonProperty("cardFront")]
        public string? CardFrontText { get; set; }
        [JsonProperty("cardBack")]
        public string? CardBackText { get; set; }
        [JsonProperty("cardTags")]
        public List<string>? Tags { get; set; }
        [JsonProperty("isFavorite")]
        public int IsFavorite { get; set; }
        [JsonProperty("studyTimes")]
        public int StudyTimes { get; set; }
        [JsonProperty("proficiency")]
        public string? Proficiency { get; set; } = ProficiencyConstants.NOT_LEARN;
        [JsonProperty("CreatedAt")]
        public DateOnly CreatedDate { get; set; }
    }
}
