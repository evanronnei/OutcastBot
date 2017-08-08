using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OutcastBot.CommandHelpers
{
    class NewBuildHelper
    {
        public static string GetBuildUrl(string buildUrl)
        {
            var grimtoolsRegex = new Regex(@"(?<=grimtools.com/calc/)(\w|\d){8}");
            var match = grimtoolsRegex.Match(buildUrl);
            
            if (match.Success)
            {
                buildUrl = $"http://www.grimtools.com/calc/{match.Value}";                
                return buildUrl;
            }
            else
            {
                return null;
            }
        }

        public static string GetTitle(string title)
        {
            if (title.Length > 100)
            {
                return title.Substring(0, 100);
            }
            else
            {
                return title;
            }
        }

        public string GetForumUrl(string forumUrl)
        {
            var gdForumRegex = new Regex(@"(?=<grimdawn.com/forums/showthread.php?t=)\d*");
            var match = gdForumRegex.Match(forumUrl);

            if (match.Success)
            {
                return $"http://www.grimdawn.com/forums/showthread.php?t={match.Value}";
            }
            else
            {
                return null;
            }
        }
    }
}
