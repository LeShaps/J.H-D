using Discord;
using J.H_D.Data.Abstracts;
using J.H_D.Data.Interfaces;
using J.H_D.Data.Interfaces.Impl;
using J.H_D.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace J.H_D.Data
{
    public delegate void MessageModifier(IUserMessage msg, string content);

    public static class JHConfig
    {
        public static Random Rand { get; private set; }

        public static string BotToken { get; private set; }
        public static bool DebugMode { get; private set; }
        public static bool IsTimerValid { get; set; }

        public static Dictionary<string, string> APIKey { get; set; }
        public static Dictionary<string, string> KitsuAuth { get; private set; }

        public static HttpClient Asker { get; private set; } = new HttpClient();
        public static HttpClient KitsuClient { get; private set; } = new HttpClient();

        public static Uri WebsiteStats { get; private set; }
        public static string WebsiteStatsToken { get; private set; }
        public static bool SendStats { get; private set; }

        public static DateTime StartingTime { get; private set; }

        public static Dictionary<ulong, Tuple<int, Response.TVSeries>> SendedSeriesEmbed { get; set; } = new Dictionary<ulong, Tuple<int, Response.TVSeries>>();
        public static Dictionary<ulong, string> GeneratedText { get; set; } = new Dictionary<ulong, string>();


        public static List<AGame> Games { get; } = new List<AGame>();
        public static ISetup[] Preloads { get; set; }
        public static GameContainer GameRunner { get; set; }

        public static Db.Db Db { private set; get; } = new Db.Db();

        public static List<ulong> AllowedBots { private set; get; } = new List<ulong>();

        public static void InitConfig()
        {
            Utilities.CheckDir("Loggers/");
            Task.Run(async () => await Db.InitAsync());

            if (!File.Exists("Loggers/Credentials.json"))
                throw new FileNotFoundException($"You must have a \"Credential.json\" file located in {AppDomain.CurrentDomain.BaseDirectory}Loggers");
            JObject ConfigurationJson = JsonConvert.DeserializeObject<JObject>(File.ReadAllText("Loggers/Credentials.json"));
            if (ConfigurationJson["botToken"] == null || ConfigurationJson["ownerId"] == null || ConfigurationJson["ownerStr"] == null)
                throw new FileNotFoundException("Missing critical informations in Credential.json, please complete mandatory informations before continue");

#if DEBUG
            DebugMode = true;
            BotToken = ConfigurationJson.Value<string>("developpmentToken") ?? ConfigurationJson.Value<string>("botToken");
#else
            DebugMode = false;
            BotToken = ConfigurationJson.Value<string>("botToken");
#endif

            WebsiteStats = new Uri(ConfigurationJson.Value<string>("WebsiteStats"));
            WebsiteStatsToken = ConfigurationJson.Value<string>("WebsiteStatsToken");

            KitsuAuth = (ConfigurationJson["kitsuCredentials"]["kitsuMail"] != null && ConfigurationJson["kitsuCredentials"]["kitsuPass"] != null) ?
                    new Dictionary<string, string>
                    {
                        {"grant_type", "password" },
                        {"username", ConfigurationJson["kitsuCredentials"].Value<string>("kitsuMail")},
                        {"password", ConfigurationJson["kitsuCredentials"].Value<string>("kitsuPass")}
                    } : null;

            StartingTime = DateTime.Now;
            var bots = ConfigurationJson["AllowedBots"].ToObject<List<ulong>>();

            APIKey = new Dictionary<string, string>
            {
                {"Tmdb", ConfigurationJson.Value<string>("MvKey") },
                {"RapidAPI", ConfigurationJson.Value<string>("RapidAPIKey") },
                {"LastFM", ConfigurationJson.Value<string>("LastFMAPIKey") },
                {"Genius", ConfigurationJson.Value<string>("GeniusApiKey") },
                {"AlphaCoders", ConfigurationJson.Value<string>("AlphaCodersApiKey") }
            };

            Rand = new Random();
            Asker.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 Jeremia");
            IsTimerValid = false;

            Preloads = new ISetup[]
            {
                new GuessNumberSetup(),
                new JamGameSetup()
            };

            GameRunner = new GameContainer();
            GameRunner.Init();
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

    public enum GameState
    {
        Prepare,
        Posting,
        Running,
        Lost
    }
}
