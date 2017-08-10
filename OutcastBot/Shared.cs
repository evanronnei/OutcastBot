using DSharpPlus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OutcastBot
{
    class Shared
    {
        public static DiscordClient Client { get; set; }
        public static List<Build> Builds { get; set; }
    }
}
