using OutcastBot.Enumerations;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
