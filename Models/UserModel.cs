using Newtonsoft.Json;

namespace AzureFuncBe.Models
{
    public class UserModel
    {
        [JsonProperty("id")]
        public required string Id { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("profile")]
        public ProfileModel? Profile { get; set; }
    }
}
