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
using System.Reflection;
using Microsoft.SqlServer.Server;
using J.H_D.Data;

namespace J.H_D.Tools
{
    class Utilities
    {
        /// <summary>
        /// Check if a directory exist, if it doesn't, create it
        /// </summary>
        /// <param name="Path">The path to test</param>
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


        /// <summary>
        /// Delete all the content inside a directory
        /// </summary>
        /// <param name="path">The path to the directory to delete content</param>
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

        /// <summary>
        /// Make the message Discord-friendly, removing the exceed of characters
        /// </summary>
        /// <param name="OriginalString">The string to test</param>
        /// <returns>A Discord-friendly message</returns>
        public static string DiscordFriendly(string OriginalString, bool Embed = false)
        {
            if (!Embed)
            {
                if (OriginalString.Length > 2000)
                {
                    string resultString = OriginalString.Substring(0, 1997);
                    resultString += "...";
                    return resultString;
                }
            }
            else
            {
                if (OriginalString.Length > 2048)
                {
                    string resultstring = OriginalString.Substring(0, 2045);
                    resultstring += "...";
                    return resultstring;
                }
            }
            return OriginalString;
        }

        /// <summary>
        /// Check if a channel is NSFW
        /// </summary>
        /// <param name="Context">A ICommandContext</param>
        /// <returns>True if the channel is NSFW</returns>
        public static bool IsChannelNSFW(ICommandContext Context)
        {
            ITextChannel channel = (ITextChannel)Context.Channel;
            return channel.IsNsfw;
        }


        /// <summary>
        /// Rotate a string upside-down
        /// </summary>
        /// <param name="OriginalString">The string to rotate</param>
        /// <returns>The rotated string</returns>
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

        /// <summary>
        /// Make a HTML text reading-friendly
        /// </summary>
        /// <param name="WebString">A HTML text</param>
        /// <returns>The text human-friendly</returns>
        public static string Clarify(string WebString)
        {
            if (WebString == null) return null;
            return GetPlainTextFromHtml(WebUtility.HtmlDecode(WebString));
        }

        /// <summary>
        /// Appends multiple strings into one
        /// </summary>
        /// <param name="Args"></param>
        /// <returns></returns>
        public static string MakeArgs(string[] Args)
        {
            if (Args == null || Args.Length == 0)
                return "";
            return string.Join(" ", Args);
        }

        /// <summary>
        /// Text escpae string to prevent bad requests
        /// </summary>
        /// <param name="Args"></param>
        /// <returns></returns>
        public static string MakeQueryArgs(string[] Args)
        {
            if (Args == null || Args.Length == 0)
                return "";
            return Uri.EscapeDataString(string.Join(" ", Args));
        }

        /// <summary>
        /// Get plain text from HTML's one using Regex
        /// </summary>
        /// <param name="htmlString">An HTML string</param>
        /// <returns>The plain text of the HTML string</returns>
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


        /// <summary>
        /// Return all the values asked in the list from the EmbedableAttribute of an object
        /// </summary>
        /// <typeparam name="T">The class of the EmbedableData object</typeparam>
        /// <param name="EmbedableData">The embedableData object</param>
        /// <param name="NeededValues">A list of values of tags you want</param>
        /// <returns></returns>
        public static Dictionary<string, string> GetEmbedAttributesValues<T>(T EmbedableData, List<string> NeededValues)
        {
            Dictionary<string, string> FoundValues = new Dictionary<string, string>();

            FieldInfo[] Fields = EmbedableData.GetType().GetFields();

            foreach (var Field in Fields)
            {
                if (NeededValues.Contains(Field.GetCustomAttribute<EmbedableAttribute>().Name))
                {
                    FoundValues.Add(Field.GetCustomAttribute<EmbedableAttribute>().Name, Field.GetValue(EmbedableData).ToString());
                }
            }

            return FoundValues;
        }

        /// <summary>
        /// Return the value asked from the Embedable data attribute
        /// </summary>
        /// <typeparam name="T">The class of the EmbedableData object</typeparam>
        /// <param name="EmbedableData">The embedableData object</param>
        /// <param name="NeededValue">The name of the Embedable Attribute field you want</param>
        /// <returns></returns>
        public static string GetEmbedAttributeValue<T>(T EmbedableData, string NeededValue)
        {
            FieldInfo[] Fields = EmbedableData.GetType().GetFields();

            foreach (var Field in Fields)
            {
                if (NeededValue == Field.GetCustomAttribute<EmbedableAttribute>().Name)
                    return Field.GetValue(EmbedableData).ToString();
            }

            return null;
        }

        /// <summary>
        /// Get memory stream from an Url
        /// </summary>
        /// <param name="Url">The url to get fetch the data from</param>
        /// <returns>A MemoryStream with the url data</returns>
        public static Stream GetStreamFromUrl(string Url)
        {
            byte[] ImageData = null;

            using (var wc = new WebClient())
                ImageData = wc.DownloadData(Url);

            return new MemoryStream(ImageData);
        }
    }
}
