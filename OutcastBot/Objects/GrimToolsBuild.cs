using OutcastBot.Enumerations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.Objects
{
    [DataContract]
    public class GrimToolsBuild
    {
        [DataMember(Name = "data")]
        public BuildData BuildData { get; set; }

        [DataMember(Name = "created_for_build")]
        public string GameVersion { get; set; }

        [DataMember(Name = "created_date")]
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

            var settings = new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true
            };
            var serializer = new DataContractJsonSerializer(typeof(GrimToolsBuild), settings);
            var response = await client.GetStreamAsync($"get_build_info.php/?id={id.Value}");
            var calc = serializer.ReadObject(response) as GrimToolsBuild;

            return calc;
        }
    }

    [DataContract]
    public class BuildData
    {
        [DataMember(Name = "bio")]
        public BuildInfo BuildInfo { get; set; }

        [DataMember(Name = "skills")]
        public Dictionary<string, int> Skills { get; set; }

        [DataMember(Name = "masteries")]
        public Dictionary<Mastery, int> Masteries { get; set; }
    }

    [DataContract]
    public class BuildInfo
    {
        [DataMember(Name = "level")]
        public int Level { get; set; }

        [DataMember(Name = "physique")]
        public int Physique { get; set; }

        [DataMember(Name = "cunning")]
        public int Cunning { get; set; }

        [DataMember(Name = "spirit")]
        public int Spirit { get; set; }
    }
}
