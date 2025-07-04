﻿using Newtonsoft.Json;
using System.Text.Json;

namespace AzureFuncBe.Models
{
    public class FolderModel
    {
        [JsonProperty("id")]
        public required string Id { get; set; }
        [JsonProperty("userId")]
        public required string UserId { get; set; }
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
