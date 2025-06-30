using Newtonsoft.Json;

namespace AzureFuncBe.DTOs
{
    public class SingleFolderResponseDTO
    {
        [JsonProperty("id")]
        public required string Id { get; set; }
        [JsonProperty("folderName")]
        public string? Name { get; set; }
        [JsonProperty("cardCount")]
        public int CardCount { get; set; } = 0;
        [JsonProperty("folderDesc")]
        public string? FolderDescription { get; set; }
        [JsonProperty("isFavorite")]
        public required bool IsFavorite { get; set; }

        [JsonProperty("createdBy")]
        public string? CreatedBy { get; set; }
        [JsonProperty("createdAt")]
        public DateOnly CreatedDate { get; set; }
    }
}
