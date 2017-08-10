using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Text;

namespace OutcastBot
{
    /// <summary>
    /// Build object
    /// </summary>
    class Build
    {
        #region Automatically Generated Properties
        public DiscordUser Author { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public ulong MessageId { get; set; }
        #endregion

        #region User Input Properties
        // required
        public string BuildUrl { get; set; }
        public string PatchVersion { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        // optional
        public DiscordAttachment HeaderImage { get; set; }
        public string ForumUrl { get; set; }
        public string VideoUrl { get; set; }
        public List<DiscordEmoji> Tags { get; set; }
        #endregion

        #region Discord Message
        public string Message
        {
            get
            {
                string message = "";

                message += $"**[{PatchVersion}] {Title}** by {Author.Mention}\n\n";
                if (HeaderImage != null) message += $"{HeaderImage.Url}\n\n";
                message += $"`Build Link:` {BuildUrl}\n";
                if (ForumUrl != null) message += $"`Forum Link:` {ForumUrl}\n";
                if (VideoUrl != null) message += $"`Video Link:` {VideoUrl}\n";
                message += $"```{Description}```";               

                return message;
            }
        }
        #endregion
    }
}
