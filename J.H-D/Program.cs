using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Globalization;

using J.H_D.Modules;
using J.H_D.Data;

namespace J.H_D
{
    class Program
    {
        public readonly DiscordSocketClient client;
        private readonly CommandService commands = new CommandService();

        private const int RefreshDelay = 300_000;
        private const int RefreshSendDelay = 6_000_000;

        private bool DebugMode;

        private static bool isTimerValid;

        // Website stats
        public static bool SendStats { private set; get; }

        // Starting Time
        public DateTime StartingTime;

        // Db
        public Db.Db db;

        public Program()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Verbose,
            });
            client.Log += LogAsync;
            // TODO: Command error handle
        }

        static async Task Main(string[] args)
        {
            try
            {
                await new Program().MainAsync().ConfigureAwait(false);
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
            => await MainAsync(null).ConfigureAwait(false);

        public async Task MainAsync(string botToken)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(RefreshDelay).ConfigureAwait(false);
                if (isTimerValid)
                    Environment.Exit(1);
            });

            await LogAsync(new LogMessage(LogSeverity.Info, "Initialization...", "Waking up J.H-D")).ConfigureAwait(false);

            db = new Db.Db();
            await db.InitAsync();

            Tools.Utilities.CheckDir("Saves");
            Tools.Utilities.CheckDir("Saves/Download");
            Tools.Utilities.CheckDir("Saves/Profiles");

            if (botToken == null)
            {
                if (!File.Exists("Loggers/Credentials.json"))
                    throw new FileNotFoundException("You must have a \"Credentials.json\" file located in " + AppDomain.CurrentDomain.BaseDirectory + "Loggers");
                dynamic json = JsonConvert.DeserializeObject(File.ReadAllText("Loggers/Credentials.json"));
                if (json.botToken == null || json.ownerId == null || json.ownerStr == null)
                    throw new FileNotFoundException("Missing informations in Credentials.json, please complete mandatory informations before continue");
                DebugMode = json.developpmentToken != null;
                botToken = DebugMode ? json.developpmentToken : json.botToken;
                // Complete informations about the owner
            }

            await LogAsync(new LogMessage(LogSeverity.Info, "Setup", "Initialising Modules...")).ConfigureAwait(false);

            await commands.AddModuleAsync<MovieModule>(null);
            await commands.AddModuleAsync<CommunicationModule>(null);
            await commands.AddModuleAsync<FChanModule>(null);
            await commands.AddModuleAsync<BooruModule>(null);
            await commands.AddModuleAsync<MusicModule>(null);
            await commands.AddModuleAsync<AnimeMangaModule>(null);

            client.MessageReceived += HandleCommandAsync;
            client.Disconnected += DisconnectedAsync;
            client.GuildAvailable += GuildJoinAsync;
            client.JoinedGuild += GuildJoinAsync;
            client.ReactionAdded += ReactionAddAsync;
            client.Disconnected += DisconnectionAsync;

            commands.CommandExecuted += CommandExectuteAsync;

            await client.LoginAsync(TokenType.Bot, botToken);
            await client.StartAsync();

            if (SendStats)
            {
                _ = Task.Run(async () =>
                {
                    while (true)
                    {
                        await Task.Delay(RefreshSendDelay).ConfigureAwait(false);
                        if (client.ConnectionState == ConnectionState.Connected)
                            throw new NotSupportedException();
                            // Update bot Status
                    }
                });
            }
            await Task.Delay(-1).ConfigureAwait(false);
        }

        private async Task DisconnectionAsync(Exception arg)
        {
            await LogAsync(new LogMessage(LogSeverity.Critical, "Program", arg.Message, null)).ConfigureAwait(false);
        }

        private async Task CheckSeriesAsync(Cacheable<IUserMessage, ulong> Message, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            if (JHConfig.SendedSeriesEmbed.ContainsKey(Message.Id))
            {
                var Used = JHConfig.SendedSeriesEmbed[Message.Id];
                MovieModule Changer = new MovieModule();

                IUserMessage Mess = await Message.DownloadAsync();

                if (Channel as ITextChannel is null)
                    await DoActionAsync(Mess.Author, 0, Module.Movie).ConfigureAwait(false);
                else
                    await DoActionAsync(Mess.Author, Mess.Channel.Id, Module.Movie).ConfigureAwait(false);

                switch (Reaction.Emote.Name)
                {
                    case "▶️":
                        JHConfig.SendedSeriesEmbed[Message.Id] = await Changer.UpdateSeriesEmbedAsync(Mess, Used, Used.Item1 + 1);
                        break;
                    case "◀️":
                        JHConfig.SendedSeriesEmbed[Message.Id] = await Changer.UpdateSeriesEmbedAsync(Mess, Used, Used.Item1 - 1);
                        break;
                    case "⏩":
                        JHConfig.SendedSeriesEmbed[Message.Id] = await Changer.UpdateSeriesEmbedAsync(Mess, Used, int.Parse(Used.Item2.SeasonNumber) - 1);
                        break;
                    case "⏪":
                        JHConfig.SendedSeriesEmbed[Message.Id] = await Changer.UpdateSeriesEmbedAsync(Mess, Used, -1);
                        break;

                    default:
                        // The user can add any other emoji so we doesn't throw
                        break;
                }
            }
        }

        private async Task CheckGeneratedTextAsync(Cacheable<IUserMessage, ulong> Message, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            if (JHConfig.GeneratedText.ContainsKey(Message.Id))
            {
                IUserMessage Mess = await Message.DownloadAsync();

                if (Channel as ITextChannel is null)
                    await DoActionAsync(Mess.Author, 0, Module.Communication).ConfigureAwait(false);
                else
                    await DoActionAsync(Mess.Author, Mess.Channel.Id, Module.Communication).ConfigureAwait(false);

                switch (Reaction.Emote.Name)
                {
                    case "🔄":
                        await new CommunicationModule().ReRollTextAsync(Mess, JHConfig.GeneratedText[Message.Id]);
                        break;

                    case "▶️":
                        await new CommunicationModule().ContinueTextAsync(Mess);
                        break;

                    default:
                        // The user can add any other emoji so we doesn't throw
                        break;
                }
            }
        }

        private async Task ReactionAddAsync(Cacheable<IUserMessage, ulong> Message, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            if (Reaction.User.Value.IsBot) {
                return;
            }

            await CheckSeriesAsync(Message, Channel, Reaction).ConfigureAwait(false);
            await CheckGeneratedTextAsync(Message, Channel, Reaction).ConfigureAwait(false);
        }

        private async Task GuildJoinAsync(SocketGuild arg)
        {
            await db.InitGuildAsync(arg);
        }

        public static async Task DoActionAsync(IUser u, ulong serverId, Module m)
        {
            if (!u.IsBot && SendStats)
                await UpdateElementAsync(new [] { new Tuple<string, string>("modules", m.ToString()) }).ConfigureAwait(false);
        }

        private Task DisconnectedAsync(Exception e)
        {
            Tools.Utilities.CheckDir("Saves/Logs");
            File.WriteAllText("Saves/Logs/" + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + ".errorlog", "Bot disconnected. Exception:\n" + e);
            return Task.CompletedTask;
        }

        private  async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg.Author.Id == client.CurrentUser.Id || arg.Author.IsBot) {
                return;
            }
            var msg = arg as SocketUserMessage;
            if (msg == null) {
                return;
            }
            bool DM = (arg.Channel as ITextChannel) is null;
            string prefix;

            int pos = 0;
            if (!DM)
            {
                prefix = db.Prefixs[(arg.Channel as ITextChannel).GuildId];
                if (msg.HasMentionPrefix(client.CurrentUser, ref pos) || (prefix != "" && msg.HasStringPrefix(prefix, ref pos)))
                {
                    var context = new SocketCommandContext(client, msg);
                    await commands.ExecuteAsync(context, pos, null);
                }
            }
            else
            {
                prefix = "jh"; //To change later to match the user configuration
                if (msg.HasMentionPrefix(client.CurrentUser, ref pos) || prefix != "" && msg.HasStringPrefix(prefix, ref pos))
                {
                    var Context = new SocketCommandContext(client, msg);
                    await commands.ExecuteAsync(Context, pos, null);
                }
            }
        }

        private async Task CommandExectuteAsync(Optional<CommandInfo> cmd, ICommandContext context, IResult result)
        {
            if (result.IsSuccess)
            {
                if (cmd.IsSpecified)
                    throw new NotSupportedException();
                if (SendStats)
                {
                    await UpdateElementAsync(new [] { new Tuple<string, string>("nbMsgs", "1") }).ConfigureAwait(false);
                    await AddErrorAsync("Ok").ConfigureAwait(false);
                    await AddCommandServsAsync(context.Guild.Id).ConfigureAwait(false);
                    if (DebugMode)
                        await LogAsync(new LogMessage(LogSeverity.Debug, "ElementUpdated", null, null)).ConfigureAwait(false);
                }
            }
        }

        private async Task AddErrorAsync(string name)
        {
            await UpdateElementAsync(new[] { new Tuple<string, string>("errors", name) }).ConfigureAwait(false);
        }

        private async Task AddCommandServsAsync(ulong name)
        {
            await UpdateElementAsync(new [] { new Tuple<string, string>("commandServs", name.ToString(CultureInfo.InvariantCulture)) }).ConfigureAwait(false);
        }

        public static async Task UpdateElementAsync(Tuple<string, string>[] elems)
        {
            Dictionary<string, string> Values = new Dictionary<string, string>
            {
                {"token", JHConfig.WebsiteStatsToken },
                {"action", "add" },
                {"name", "Jeremia" }
            };
            foreach (var elem in elems)
            {
                Values.Add(elem.Item1, elem.Item2);
            }
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, JHConfig.WebsiteStats);
            msg.Content = new FormUrlEncodedContent(Values);

            await JHConfig.Asker.SendAsync(msg);
        }

        private Task LogAsync(LogMessage msg)
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

                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.WriteLine(msg);
            Console.ForegroundColor = cc;
            return Task.CompletedTask;
        }
    }
}
