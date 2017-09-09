using OutcastBot.Enumerations;
using System;
using System.Collections.Generic;

namespace OutcastBot.Objects
{
    public class GrimToolsCalc
    {
        public CalcData Data { get; set; }
        public string Created_For_Build { get; set; }
        public DateTime Created_Date { get; set; }
    }

    public class CalcData
    {
        public CalcBio Bio { get; set; }
        public Dictionary<string, int> Skills { get; set; }
        public Dictionary<Mastery, int> Masteries { get; set; }
    }

    public class CalcBio
    {
        public int Level { get; set; }
        public int Physique { get; set; }
        public int Cunning { get; set; }
        public int Spirit { get; set; }
    }
}
