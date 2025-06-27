using Newtonsoft.Json;

namespace AzureFuncBe.Models
{
    public class UserModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
