using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Text;

namespace OutcastBot
{
    class Build
    {
        public DiscordUser Author { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ForumUrl { get; set; }
        public string VideoUrl { get; set; }
    }
}
