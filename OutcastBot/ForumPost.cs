using DSharpPlus.Entities;
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace OutcastBot
{
    public class ForumPost
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string AuthorAvatarUrl { get; set; }
        public int ReplyCount { get; set; }
        public DateTimeOffset PostDate { get; set; }

        private string _url;
        private string _content;

        public ForumPost(string url)
        {
            _url = url;
            using (var client = new WebClient()) _content = client.DownloadString(_url);
            Parse();
        }

        public DiscordEmbed GetEmbed()
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = this.Title,
                Url = _url
            };

            embed.WithAuthor(Author, null, AuthorAvatarUrl);

            embed.AddField("Replies", ReplyCount.ToString());

            embed.WithTimestamp(PostDate);

            return embed.Build();
        }

        private void Parse()
        {
            GetTitle();
            GetAuthor();
            GetAuthorAvatarUrl();
            GetReplyCount();
            GetPostDate();
        }

        private void GetTitle()
        {
            var title = new Regex(@"(?<=<meta\sname=""description""\scontent=""\s)([^(""\s/?)]|\s)*(?=""\s/>)")
                .Match(_content);
            Title = title.Value.Replace("&amp;", "&");
        }

        private void GetAuthor()
        {
            var author = new Regex(@"(?<=<a\sclass=""bigusername""\shref=""member.php\?s=[a-z0-9]*&amp;u=\d*"">)[^(</a>)]*(?=</a>)")
                .Match(_content);
            Author = author.Value;
        }

        private void GetAuthorAvatarUrl()
        {
            var avatarUrl = new Regex(@"(?<=<td\sclass=""alt2""><a\shref=""member.php\?s=[a-z0-9]*&amp;u=\d*""><img\ssrc="")(image\.php\?s=[a-z0-9]*&amp;u=\d*&amp;dateline=\d*)")
                .Match(_content);
            if (avatarUrl.Success)
            {
                var clean = new Regex(@"(s=[a-z0-9]*&amp;)").Match(avatarUrl.Value);
                var cleanedUrl = avatarUrl.Value.Replace(clean.Value, "");
                AuthorAvatarUrl = $"http://www.grimdawn.com/forums/{cleanedUrl.Replace("&amp;", "&")}";
            }
        }

        private void GetReplyCount()
        {
            var replyCount = new Regex(@"(?<=<td\sclass=""alt2""><span\sclass=""smallfont""\stitle=""Showing\sresults\s\d+\sto\s\d+\sof\s)\d+")
                .Match(_content);
            ReplyCount = Convert.ToInt32(replyCount.Value) - 1;
        }

        private void GetPostDate()
        {
            var postDate = new Regex(@"\d{2}-\d{2}-\d{4},\s\d{2}:\d{2}\s\w{2}")
                .Match(_content);
            PostDate = DateTimeOffset.ParseExact($"{postDate.Value}+0", "MM-dd-yyyy, hh:mm ttz", null);
        }
    }
}
