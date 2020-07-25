using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using J.H_D.Modules;
using Newtonsoft.Json;
using RethinkDb.Driver.Ast;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace J.H_D
{
    class Program
    {
        public readonly DiscordSocketClient client;
        private readonly CommandService commands = new CommandService();
        public static Program p;
        public System.Random rand;
        public string TmDbKey;
        public string RapidAPIKey;
        public string LastFMKey;
        public HttpClient Asker;

        private bool DebugMode;

        private static bool isTimerValid;

        // Website stats
        private string WebsiteStats, WebsiteStatsToken;
        public string WebsiteUpload { get; private set; }
        public string WebsiteUrl { private set; get; }
        public bool SendStats { private set; get; }

        // Starting Time
        public DateTime StartingTime;

        // Db
        public Db.Db db;

        public Dictionary<string, string> KitsuAuth;

        public enum Module
        {
            AnimeManga,
            Forum,
            Vn,
            Movie,
            Booru
        }

        public Program()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Verbose,
            });
            client.Log += Log;
            // TODO: Command error handle
        }

        static async Task Main(string[] args)
        {
            try
            {
                await new Program().MainAsync();
            }
            catch(FileNotFoundException)
            {
                throw;
            }
            catch(Exception)
            {
                if (Debugger.IsAttached)
                    throw;
            }
        }

        public async Task MainAsync()
            => await MainAsync(null, 0);

        public async Task MainAsync(string botToken, ulong inamiId)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(300000);
                if (isTimerValid)
                    Environment.Exit(1);
            });
            p = this;

            await Log(new LogMessage(LogSeverity.Info, "Initialization...", "Waking up J.H-D"));

            db = new Db.Db();
            await db.InitAsync();
            rand = new System.Random();

            Tools.Utilities.CheckDir("Saves");
            Tools.Utilities.CheckDir("Saves/Download");
            Tools.Utilities.CheckDir("Saves/Profiles");

            if (botToken == null)
            {
                if (!File.Exists("Loggers/Credentials.json"))
                    throw new FileNotFoundException("You must have a \"Credentials.json\" file located in " + AppDomain.CurrentDomain.BaseDirectory + "Loggers");
                dynamic json = JsonConvert.DeserializeObject(File.ReadAllText("Loggers/Credentials.json"));
                if (json.botToken == null || json.ownerId == null || json.ownerStr == null)
                    throw new NullReferenceException("Missing informations in Credentials.json, please complete mandatory informations before continue");
                if (json.developpmentToken != null)
                    DebugMode = true;
                botToken = json.botToken;
                // Complete informations about the owner

                if (json.kitsuCredentials.kitsuMail != null && json.kitsuCredentials.kitsuPass != null)
                {
                    KitsuAuth = new Dictionary<string, string>()
                    {
                        {"grant_type", "password" },
                        {"username", (string)json.kitsuCredentials.KitsuMail },
                        {"password", (string)json.kitsuCredentails.KitsuKey }
                    };
                }
                else
                    KitsuAuth = null;

                Asker = new HttpClient();

                await InitServices(json);
            }
            await Log(new LogMessage(LogSeverity.Info, "Setup", "Initialising Modules..."));

            await commands.AddModuleAsync<MovieModule>(null);
            await commands.AddModuleAsync<CommunicationModule>(null);
            await commands.AddModuleAsync<FChanModule>(null);
            await commands.AddModuleAsync<BooruModule>(null);
            await commands.AddModuleAsync<MusicModule>(null);

            client.MessageReceived += HandleCommandAsync;
            client.Disconnected += Disconnected;
            client.GuildAvailable += GuildJoin;
            client.JoinedGuild += GuildJoin;
            commands.CommandExecuted += CommandExectute;

            await client.LoginAsync(TokenType.Bot, botToken);
            StartingTime = DateTime.Now;
            await client.StartAsync();

            if (SendStats)
            {
                _ = Task.Run(async () =>
                {
                    for (; ; )
                    {
                        await Task.Delay(6000000);
                        if (client.ConnectionState == ConnectionState.Connected)
                            throw new NotImplementedException();
                            // Update bot Status
                    }
                });
            }
            await Task.Delay(-1);
        }

        private async Task GuildJoin(SocketGuild arg)
        {
            await db.InitGuild(arg);
        }

        private async Task InitServices(dynamic json)
        {
            TmDbKey = json.MvKey;
            RapidAPIKey = json.RapidAPIKey;
            LastFMKey = json.LastFMAPIKey;
        }

        public async Task DoAction(IUser u, ulong serverId, Module m)
        {
            if (!u.IsBot && SendStats)
                await UpdateElement(new Tuple<string, string>[] { new Tuple<string, string>("modules", m.ToString()) });
        }

        private Task Disconnected(Exception e)
        {
            Tools.Utilities.CheckDir("Saves/Logs");
            File.WriteAllText("Saves/Logs/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".errorlog", "Bot disconnected. Exception:\n" + e.ToString());
            return Task.CompletedTask;
        }

        private  async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg.Author.Id == client.CurrentUser.Id || arg.Author.IsBot) return;
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            int pos = 0;
            string prefix = db.Prefixs[(arg.Channel as ITextChannel).GuildId];
            if (msg.HasMentionPrefix(client.CurrentUser, ref pos) || (prefix != "" && msg.HasStringPrefix(prefix, ref pos)))
            {
                var context = new SocketCommandContext(client, msg);
                if (!((IGuildUser)await context.Channel.GetUserAsync(client.CurrentUser.Id)).GetPermissions((IGuildChannel)context.Channel).EmbedLinks)
                {
                    // Make error if have not the rights to Embed links
                }
                await commands.ExecuteAsync(context, pos, null);
            }
        }

        private async Task CommandExectute(Optional<CommandInfo> cmd, ICommandContext context, IResult result)
        {
            if (result.IsSuccess)
            {
                DateTime dt = DateTime.UtcNow;
                var msg = context.Message;
                if (cmd.IsSpecified)
                    throw new NotImplementedException();
                if (SendStats)
                {
                    await UpdateElement(new Tuple<string, string>[] { new Tuple<string, string>("nbMsgs", "1") });
                    await AddError("Ok");
                    await AddCommandServs(context.Guild.Id);
                }
            }
        }

        private async Task AddError(string name)
        {
            await UpdateElement(new Tuple<string, string>[] { new Tuple<string, string>("errors", name) });
        }

        private async Task AddCommandServs(ulong name)
        {
            await UpdateElement(new Tuple<string, string>[] { new Tuple<string, string>("commandServs", name.ToString()) });
        }

        public async Task UpdateElement(Tuple<string, string>[] elems)
        {
            HttpClient client = new HttpClient();
            Dictionary<string, string> Values = new Dictionary<string, string>
            {
                {"token", WebsiteStatsToken },
                {"action", "add" },
                {"name", "Jeremia" }
            };
            foreach (var elem in elems)
            {
                Values.Add(elem.Item1, elem.Item2);
            }
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, WebsiteStats);
            msg.Content = new FormUrlEncodedContent(Values);

            try
            {
                await client.SendAsync(msg);
            }
            catch (HttpRequestException) { }
            catch (TaskCanceledException) { }
        }

        private Task Log(LogMessage msg)
        {
            var cc = Console.ForegroundColor;
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine(msg);
            Console.ForegroundColor = cc;
            return Task.CompletedTask;
        }
    }
}
