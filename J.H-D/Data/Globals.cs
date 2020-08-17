using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace J.H_D.Data
{
    public delegate void MessageModifier(IUserMessage msg, string content);

    public static class JHConfig
    {
        public static Random rand;

        public static string TmdbKey;
        public static string RapidAPIKey;
        public static string LastFMKey;
        public static Dictionary<string, string> KitsuAuth;

        public static HttpClient Asker = new HttpClient();
        public static HttpClient KitsuClient = new HttpClient();

        public static Uri WebsiteStats;
        public static string WebsiteStatsToken;
        public static bool SendStats;

        public static DateTime StartingTime;

        public static Dictionary<ulong, Tuple<int, Response.TVSeries>> SendedSeriesEmbed = new Dictionary<ulong, Tuple<int, Response.TVSeries>>();
        public static Dictionary<ulong, string> GeneratedText = new Dictionary<ulong, string>();

        static JHConfig()
        {
            Tools.Utilities.CheckDir("Loggers/");

            dynamic ConfigurationJson = JsonConvert.DeserializeObject(File.ReadAllText("Loggers/Credentials.json"));

            WebsiteStats = new Uri((string)ConfigurationJson.WebsiteStats);
            WebsiteStatsToken = ConfigurationJson.WebsiteStatsToken;

            KitsuAuth = (ConfigurationJson.kitsuCredentials.kitsuMail != null && ConfigurationJson.kitsuCredentials.kitsuPass != null) ?
                    new Dictionary<string, string>
                    {
                        {"grant_type", "password" },
                        {"username", ConfigurationJson.kitsuCredentials.KitsuMail as string},
                        {"password", ConfigurationJson.kitsuCredentails.KitsuKey as string}
                    } : null;

            StartingTime = DateTime.Now;

            TmdbKey = ConfigurationJson.MvKey;
            RapidAPIKey = ConfigurationJson.RapidAPIKey;
            LastFMKey = ConfigurationJson.LastAMAPIKey;

            rand = new Random();
            Asker.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 Jeremia");
        }
    }

    public enum Module
    {
        AnimeManga,
        Forum,
        Movie,
        Booru,
        Communication,
        Music
    }
}
