using Newtonsoft.Json;
using System.Collections.Generic;

namespace OutcastBot.Objects
{
    [JsonObject]
    public class ApplicationSettings
    {
        [JsonProperty]
        public string Token { get; set; }

        [JsonProperty]
        public string CommandPrefix { get; set; }

        [JsonProperty]
        public List<string> QuotableChannels { get; set; }
    }
}
