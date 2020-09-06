using Discord;
using Discord.Commands;
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
        public static Random Rand { get; private set; }

        public static string BotToken { get; private set; }
        public static bool DebugMode { get; private set; }
        public static bool IsTimerValid { get; set; }

        public static string TmdbKey { get; private set; }
        public static string RapidApiKey { get; private set; }
        public static string LastFMKey { get; private set; }
        public static Dictionary<string, string> KitsuAuth { get; private set; }

        public static HttpClient Asker { get; private set; } = new HttpClient();
        public static HttpClient KitsuClient { get; private set; } = new HttpClient();

        public static Uri WebsiteStats { get; private set; }
        public static string WebsiteStatsToken { get; private set; }
        public static bool SendStats { get; private set; }

        public static DateTime StartingTime { get; private set; }

        public static Dictionary<ulong, Tuple<int, Response.TVSeries>> SendedSeriesEmbed { get; set; } = new Dictionary<ulong, Tuple<int, Response.TVSeries>>();
        public static Dictionary<ulong, string> GeneratedText { get; set; } = new Dictionary<ulong, string>();

        public static void InitConfig()
        {
            Tools.Utilities.CheckDir("Loggers/");

            if (!File.Exists("Loggers/Credentials.json"))
                throw new FileNotFoundException($"You must have a \"Credential.json\" file located in {AppDomain.CurrentDomain.BaseDirectory}Loggers");
            dynamic ConfigurationJson = JsonConvert.DeserializeObject(File.ReadAllText("Loggers/Credentials.json"));
            if (ConfigurationJson.botToken == null || ConfigurationJson.ownerId == null || ConfigurationJson.ownerStr == null)
                throw new FileNotFoundException("Missing critical informations in Credential.json, please complete mandatory informations before continue");
            DebugMode = ConfigurationJson.developpmentToken != null;
            BotToken = DebugMode ? ConfigurationJson.developpmentToken : ConfigurationJson.botToken;

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
            RapidApiKey = ConfigurationJson.RapidAPIKey;
            LastFMKey = ConfigurationJson.LastAMAPIKey;

            Rand = new Random();
            Asker.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 Jeremia");
            IsTimerValid = false;
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

    [Flags]
    public enum Warnings
    {
        NSFW = 1,
        Spoilers = 2,
        RequireAuthorization = 4,
        None = 8
    }

    public enum ParameterType
    {
        Mandatory,
        Optional
    }
}
