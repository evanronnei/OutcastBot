using Newtonsoft.Json;

namespace OutcastBot.Objects
{
    [JsonObject]
    public class ApplicationSettings
    {
        [JsonProperty]
        public string Token { get; set; }

        [JsonProperty]
        public string CommandPrefix { get; set; }
    }
}
