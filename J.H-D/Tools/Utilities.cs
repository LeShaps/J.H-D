using System;
﻿using Discord;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Discord.Commands;

using J.H_D.Data;
using J.H_D.Data.Exceptions;
using J.H_D.Modules;
using J.H_D.Data.Extensions;

namespace J.H_D.Tools
{
    public static class Utilities
    {
        private const int DiscordMessageCharacterLimit = 2000;
        private const int DiscordEmbedMessageCharacterLimit = 2048;

        /// <summary>
        /// Check if a directory exist, if it doesn't, create it
        /// </summary>
        /// <param name="Path">The path to test</param>
        public static void CheckDir(string Path)
        {
            if (Path == null)
                throw new ArgumentNullException(Path);
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
        public static string DiscordFriendly(string OriginalString)
        {
            if (OriginalString.Length > DiscordMessageCharacterLimit)
            {
                string resultString = OriginalString.Substring(0, DiscordMessageCharacterLimit - 3);
                resultString = $"{resultString}...";
                return resultString;
            }

            return OriginalString;
        }

        public static string DiscordEmbedFriendly(string OriginalString)
        {
            if (OriginalString.Length > DiscordEmbedMessageCharacterLimit)
            {
                string resultString = OriginalString.Substring(0, DiscordEmbedMessageCharacterLimit - 3);
                resultString = $"{resultString}...";
                return resultString;
            }

            return OriginalString;
        }

        /// <summary>
        /// Check if a channel is NSFW
        /// </summary>
        /// <param name="Context">A ICommandContext</param>
        /// <returns>True if the channel is NSFW</returns>
        public static bool IsChannelNsfw(ICommandContext Context)
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
            StringBuilder bld = new StringBuilder();
            foreach (char c in OriginalString) {
                bld.Append(Normal.Contains(c) ? Rotated[Normal.IndexOf(c)] : c);
            }
            newString = bld.ToString();

            return newString;
        }

        /// <summary>
        /// Make a HTML text reading-friendly
        /// </summary>
        /// <param name="WebString">A HTML text</param>
        /// <returns>The text human-friendly</returns>
        public static string Clarify(string WebString)
        {
            if (WebString == null) {
                return null;
            }
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
            if (htmlString == null) {
                return null;
            }

            const string htmlTagPattern = "<.*?>";
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
        public static IDictionary<string, string> GetEmbedAttributesValues<T>(T EmbedableData, ICollection<string> NeededValues)
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
        public static Stream GetStreamFromUrl(Uri Url)
        {
            byte[] ImageData = null;

            using (var wc = new WebClient())
                ImageData = wc.DownloadData(Url);

            return new MemoryStream(ImageData);
        }

        public static string GetHTMLSection(string Source, string Start, string End)
        {
            if (HasNullOrEmpty(Source, Start, End))
                throw new ArgumentNullException(Source);

            string[] Splits = Source.Split(Start);

            if (Splits.Length <= 1)
                throw new SectionNotFoundException();

            return Splits[1].Split(End).FirstOrDefault();
        }

        public static string GetHTMLSection(string Source, string Start, string End, int Section)
        {
            if (HasNullOrEmpty(Source, Start, End))
                throw new ArgumentNullException(Source);

            string[] Splits = Source.Split(Start);

            if (Splits.Length <= 1)
                throw new SectionNotFoundException();

            return Splits[Section].Split(End).FirstOrDefault();
        }

        public static bool HasNullOrEmpty(params string[] Args)
        {
            foreach (string s in Args)
            {
                if (s.IsNullOrEmpty())
                    return true;
            }

            return false;
        }

        public static string StandardUppercase(string ToUp)
        {
            char UppedChar = char.ToUpperInvariant(ToUp[0]);
            string Lowered = ToUp.Skip(1).ToString().ToLowerInvariant();

            return $"{UppedChar}{Lowered}";
        }

        public static async Task CallHelper(string CommandName)
        {
            CommunicationModule Module = new CommunicationModule();

            await Module.DisplayCommandInfosAsync(CommandName);
        }
    }
}
