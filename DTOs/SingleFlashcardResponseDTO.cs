using AzureFuncBe.Models;
using Newtonsoft.Json;

namespace AzureFuncBe.DTOs
{
    public class SingleFlashcardResponseDTO
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
        public bool IsFavorite { get; set; }
        [JsonProperty("studyTimes")]
        public int StudyTimes { get; set; }
        [JsonProperty("proficiency")]
        public string? Proficiency { get; set; }
        [JsonProperty("CreatedAt")]
        public DateOnly CreatedDate { get; set; }
    }
}
