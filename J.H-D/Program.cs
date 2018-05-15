using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;


using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Net;
using System.IO;

namespace J.H_D
{
    public class Program : ModuleBase
    {
        public static void Main(string[] args)
        {
            Console.Title = "J.H-DiscordBot v.2";
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        private readonly DiscordSocketClient client;

        private readonly IServiceCollection map = new ServiceCollection();
        private CommandService commands = new CommandService();
        private IServiceProvider services;

        public static Program p;
        static public WebClient malAsker;
        public Askers callers;
        public Random rand;

        private Program()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
            });
            client.Log += Log;
            commands.Log += Log;
        }

        private static Task Logger(LogMessage message)
        {
            var cc = Console.ForegroundColor;
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message}");
            Console.ForegroundColor = cc;

            return Task.CompletedTask;
        }

        private async Task MainAsync()
        {
            p = this;
            callers = new Askers();
            malAsker = callers.init_malCaller(malAsker);
            rand = new Random();

            await commands.AddModuleAsync<Program>();
            await commands.AddModuleAsync<MalModule>();
            await commands.AddModuleAsync<UnsafeModule>();
            await commands.AddModuleAsync<ForumsModule>();
            await commands.AddModuleAsync<MovieModule>();
            
            client.MessageReceived += HandleCommandAsync;

            await client.LoginAsync(TokenType.Bot, File.ReadAllLines("Logers/token.txt")[0]);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        public static string removeSymbols(string unclean)
        {
            unclean = unclean.Replace("[i]", "*");
            unclean = unclean.Replace("[/i]", "*");
            unclean = unclean.Replace("&lt;br /&gt;", Environment.NewLine);
            unclean = unclean.Replace("<br>", Environment.NewLine);
            unclean = unclean.Replace("&amp;mdash;", " : ");
            unclean = unclean.Replace("mdash;", "—");
            unclean = unclean.Replace("&quot;", "\"");
            unclean = unclean.Replace("&amp;", "&");
            unclean = unclean.Replace("&#039;", "'");
            unclean = unclean.Replace("&amp;ccedil;", "ç");
            unclean = unclean.Replace("[Written by MAL Rewrite]", "");
            return (unclean);
        }

        public static string makeArgs(string[] args)
        {
            if (args.Length == 0)
                return (null);
            string finalStr = args[0];
            for (int i = 1; i < args.Length; i++)
            {
                finalStr += " " + args[i];
            }
            return (finalStr);
        }

        public static string getInfos(string search,  string initial, char endcarac)
        {
            int use = 0;
            string final = null;
            
            foreach (char c in initial)
            {
                if (use == search.Length)
                {
                    if (c == endcarac)
                        break;
                    final += c;
                }
                else
                {
                    if (c == search[use])
                    {
                        use++;
                    }
                    else
                        use = 0;
                }
            }
            return (final);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            int pos = 0;
            if (msg.HasMentionPrefix(client.CurrentUser, ref pos))
            {
                var context = new SocketCommandContext(client, msg);

                var result = await commands.ExecuteAsync(context, pos, services);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
            var comcontext = new SocketCommandContext(client, msg);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}