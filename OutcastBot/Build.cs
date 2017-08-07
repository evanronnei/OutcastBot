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
        public DiscordAttachment HeaderImage { get; set; }
        public string VideoUrl { get; set; }
        public string Message { get
            {
                string message = "";

                message += $"**{Title}** by {Author.Mention}\n";
                message += $"{Url}\n";
                if (ForumUrl != null) message += $"Forum Link: {ForumUrl}\n";
                if (HeaderImage != null) message += $"{HeaderImage.Url}\n";
                message += $"```{Description}```";
                if (VideoUrl != null) message += $"{VideoUrl}\n";

                return message;
            } }
    }
}
