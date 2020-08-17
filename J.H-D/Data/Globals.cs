using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace J.H_D.Data
{
    public delegate void MessageModifier(IUserMessage msg, string content);

    public static class JHConfig
    {
        public static Random rand { get; private set; }

        public static string TmdbKey { get; private set; }
        public static string RapidAPIKey { get; private set; }
        public static string LastFMKey { get; private set; }
        public static Dictionary<string, string> KitsuAuth { get; private set; }

        public static HttpClient Asker { get; private set; } = new HttpClient();
        public static HttpClient KitsuClient { get; private set; } = new HttpClient();

        public static Uri WebsiteStats { get; private set; }
        public static string WebsiteStatsToken { get; private set; }
        public static bool SendStats { get; private set; }

        public static DateTime StartingTime { get; private set; }

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
