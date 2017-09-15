using Newtonsoft.Json;
using OutcastBot.Enumerations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Objects
{
    [JsonObject]
    public class GrimToolsBuild
    {
        [JsonProperty("data")]
        public BuildData BuildData { get; set; }

        [JsonProperty("created_for_build")]
        public string GameVersion { get; set; }

        [JsonProperty("created_date")]
        public string CreatedDate { get; set; }

        public static async Task<GrimToolsBuild> GetGrimToolsBuildAsync(string buildUrl)
        {
            var id = new Regex(@"(?<=http://www.grimtools.com/calc/)[a-zA-Z0-9]{8}").Match(buildUrl);

            var client = new HttpClient
            {
                BaseAddress = new Uri("http://www.grimtools.com/")
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetStringAsync($"get_build_info.php/?id={id.Value}");
            var calc = JsonConvert.DeserializeObject<GrimToolsBuild>(response);

            return calc;
        }
    }

    [JsonObject]
    public class BuildData
    {
        [JsonProperty("bio")]
        public BuildInfo BuildInfo { get; set; }

        [JsonProperty("skills")]
        public Dictionary<string, int> Skills { get; set; }

        [JsonProperty("masteries")]
        public Dictionary<Mastery, int> Masteries { get; set; }
    }

    [JsonObject]
    public class BuildInfo
    {
        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("physique")]
        public int Physique { get; set; }

        [JsonProperty("cunning")]
        public int Cunning { get; set; }

        [JsonProperty("spirit")]
        public int Spirit { get; set; }
    }
}
