using Newtonsoft.Json;
using OutcastBot.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Objects
{
    [JsonObject]
    public class GrimToolsCalc
    {
        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("created_for_build")]
        public string GameVersion { get; set; }

        [JsonProperty("created_date")]
        public string CreatedDate { get; set; }

        public static async Task<GrimToolsCalc> GetGrimToolsCalcAsync(string calcUrl)
        {
            var id = new Regex(@"(?<=http://www.grimtools.com/calc/)[a-zA-Z0-9]{8}").Match(calcUrl);

            var client = new HttpClient
            {
                BaseAddress = new Uri("http://www.grimtools.com/")
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetStringAsync($"get_build_info.php/?id={id.Value}");
            var calc = JsonConvert.DeserializeObject<GrimToolsCalc>(response);

            return calc;
        }

        public Mastery GetMasteryCombination()
        {
            return Data.Masteries.Keys.Aggregate((masteryOne, masteryTwo) => masteryOne | masteryTwo);
        }
    }

    [JsonObject]
    public class Data
    {
        [JsonProperty("bio")]
        public Info Info { get; set; }

        [JsonProperty("skills")]
        public Dictionary<string, int> Skills { get; set; }

        [JsonProperty("masteries")]
        public Dictionary<Mastery, int> Masteries { get; set; }
    }

    [JsonObject]
    public class Info
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
