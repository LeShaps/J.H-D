using Discord;
using RethinkDb.Driver.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using Discord.Commands;

namespace J.H_D.Tools
{
    class Utilities
    {
        public static void CheckDir(string Path)
        {
            string[] list = Path.Split('/');
            if (list[0].Length == 0)
                return;

            for (int i = 0; i <= list.Length - 1; i++)
            {
                if (!Directory.Exists(list[i]))
                    Directory.CreateDirectory(list[i]);
                if (i + 1 < list.Length)
                    list[i + 1] = list[i] + "/" + list[i + 1];
            }
        }

        public static void DeleteDirectoryContent(string path)
        {
            foreach (string f in Directory.GetFiles(path))
                File.Delete(f);
            foreach (string d in Directory.GetDirectories(path))
            {
                DeleteDirectoryContent(d);
                Directory.Delete(d);
            }
        }

        public static string DiscordFriendly(string OriginalString)
        {
            if (OriginalString.Length > 2000)
            {
                string resultString = OriginalString.Substring(1997);
                resultString += "...";
                return resultString;
            }
            return OriginalString;
        }

        public static bool IsChannelNSFW(ICommandContext Context)
        {
            ITextChannel channel = (ITextChannel)Context.Channel;
            return channel.IsNsfw;
        }

        public static string RotateString(string OriginalString)
        {
            string Normal = "abcdefghijklmnopqrstuvwxyz_,;.?!/\\'";
            string Rotated = "ɐqɔpǝɟbɥıظʞןɯuodbɹsʇnʌʍxʎz‾'؛˙¿¡/\\,";

            Normal += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Rotated += "∀qϽᗡƎℲƃHIſʞ˥WNOԀὉᴚS⊥∩ΛMXʎZ";

            Normal += "0123456789";
            Rotated += "0ƖᄅƐㄣϛ9ㄥ86";

            string newString = "";
            foreach (char c in OriginalString)
            {
                if (Normal.Contains(c))
                    newString += Rotated[Normal.IndexOf(c)];
                else
                    newString += c;
            }

            return newString;
        }

        public static string Clarify(string WebString)
        {
            if (WebString == null) return null;
            return GetPlainTextFromHtml(WebUtility.HtmlDecode(WebString));
        }

        public static string MakeArgs(string[] Args)
        {
            if (Args == null || Args.Length == 0)
                return "";
            return string.Join(" ", Args);
        }

        public static string MakeQueryArgs(string[] Args)
        {
            if (Args == null || Args.Length == 0)
                return "";
            return Uri.EscapeDataString(string.Join(" ", Args));
        }

        public static string GetPlainTextFromHtml(string htmlString)
        {
            if (htmlString == null) return null;

            string htmlTagPattern = "<.*?>";
            var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            htmlString = regexCss.Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);

            return htmlString;
        }
    }
}
