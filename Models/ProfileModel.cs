using Newtonsoft.Json;

namespace AzureFuncBe.Models
{
    public class ProfileModel
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("userId")]
        public string? UserId { get; set; }

        [JsonProperty("numCards")]
        public int NumCards { get; set; } = 0;

        [JsonProperty("numQuizzes")]
        public int NumQuizzes { get; set; } = 0;
    }
}
