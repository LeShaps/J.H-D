using System;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using J.H_D.Tools;
using J.H_D.Data;
using J.H_D.Minions.Websites;
using J.H_D.Minions.Infos;
using System.Text.RegularExpressions;
using J.H_D.Data.Extensions;
using System.Data;

namespace J.H_D.Modules
{
    class CommunicationModule : ModuleBase
    {
        private readonly int MaxIterationNumber = 10;

        [Command("Hey !")]
        public async Task ReplytoAsync()
        {
            await ReplyAsync("Hi !");
        }

        [Command("Rotate")]
        [Help("Communication", "Rotate a given text")]
        [Parameter("Text", "The text to rotate", ParameterType.Mandatory)]
        public async Task RotateThenRespondAsync([Remainder]string Args)
        {
            await ReplyAsync(Utilities.RotateString(Args));
        }

        [Command("Clarify")]
        [Help("Communication", "Convert an HTML text to human-readable text")]
        [Parameter("Text", "The HTML text to clarify", ParameterType.Mandatory)]
        public async Task ClarifyThenRespondAsync([Remainder]string Args)
        {
            await ReplyAsync(Utilities.GetPlainTextFromHtml(Args));
        }

        [Command("Motive"), Alias("Inspire")]
        [Help("Communication", "Generate a motivational poster using Inspirobot", Warnings.NSFW)]
        [Parameter("Clean", "If you want the image to be send as a file, and not in an embed, send \"Clean\"", ParameterType.Optional)]
        public async Task MotiveAsync(params string[] Args)
        {
            string cleanArgs = Utilities.MakeArgs(Args).ToLower();
            bool Natural = false;

            Natural = cleanArgs == "clean";

            var Result = await InspirobotMinion.FeelInspirationAsync();

            switch (Result.Error)
            {
                case Error.InspirationnalError.Communication:
                    await ReplyAsync("I'm not feeling too inspired today");
                    break;

                case Error.InspirationnalError.None:
                    if (!Natural)
                        await ReplyAsync("", false, BuildInspirobotEmbed(new Uri(Result.Answer)));
                    else
                    {
                        string file = await PureImageAsync(new Uri(Result.Answer)).ConfigureAwait(false);
                        await Context.Channel.SendFileAsync(file);
                        File.Delete(file);
                    }
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        [Command("Say")]
        public async Task ReSay([Remainder]string Phrase)
        {
            await ReplyAsync(Phrase);
        }

        [Command("Define")]
        [Help("Communication", "Define a word (definition from Urban Dictionnary)", Warnings.NSFW | Warnings.Spoilers)]
        [Parameter("Word", "The word to define", ParameterType.Mandatory)]
        public async Task FindDefinitionAsync(params string[] Args)
        {
            string FinalArgs = Utilities.MakeQueryArgs(Args);

            if (!Utilities.IsChannelNsfw(Context)) {
                await ReplyAsync("Nah, there's some things I can't tell you here, try in a NSFW channel");
                return;
            }

            var Result = await UrbanDictionaryMinion.SearchForWordAsync(FinalArgs);

            switch (Result.Error)
            {
                case Error.Urban.WordNotFound:
                    await ReplyAsync("Sorry, even internet can't help you on this one");
                    break;

                case Error.Urban.None:
                    await ReplyAsync("", false, BuildDefinition((Response.UrbanDefinition)Result.Answer));
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        [Command("Generate", RunMode = RunMode.Async)]
        [Help("Communication", "Generate a text using AI", Warnings.NSFW)]
        [Parameter("Starting text", "The text to begin with", ParameterType.Mandatory)]
        public async Task GenerateAsync([Remainder]string sentence)
        {
            var msg = await StartWaitAsync(sentence).ConfigureAwait(false);
            var Response = await GeneratorMinion.CompleteAsync(sentence, msg, MessageUpdaterAsync);

            switch (Response.Error)
            {
                case Error.Complete.Help:
                    await DisplayCommandInfosAsync("Generate").ConfigureAwait(false);
                    break;

                case Error.Complete.Connection:
                    await ReplyAsync("Can't connect to textsynth");
                    break;

                case Error.Complete.None:
                    await msg.ModifyAsync(x => x.Embed = CreateTextEmbed(Response.Answer.Content));
                    JHConfig.GeneratedText.Add(msg.Id, sentence);
                    await msg.AddReactionsAsync(new[] { new Emoji("🔄"), new Emoji("▶️") });
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        [Command("Help")]
        [Help("Communication", "Display all commands")]
        public async Task DisplayHelpAsync()
        {
            var Methods = GetAvailablesCommands();

            EmbedBuilder BuildHelp = new EmbedBuilder
            {
                Color = new Color(214, 201, 45),
                Title = "Commands",
                Fields = GetCommandsName(Methods),
                Footer = new EmbedFooterBuilder
                {
                    Text = "Use Command Info + Command Name to get more infos about a listed command"
                }
            };

            await ReplyAsync("", false, BuildHelp.Build());
        }

        [Command("Set prefix")]
        public async Task UpdatePrefixForServer([Remainder]string prefix)
        {
            await JHConfig.Db.UpdateGuildPrefixAsync(Context.Guild, prefix);
            await ReplyAsync("Prefix updated for this guild, the new prefix is " + JHConfig.Db.Prefixs[Context.Guild.Id]);
        }

        [Command("Transform")]
        public async Task ChangeRoll([Remainder] string Expression)
        {
            Console.WriteLine("Entering Transform");
            Regex reg = new Regex("([0-9]+)?d([0-9]+)");
            string result = Regex.Replace(Expression, "([0-9]+)?d([0-9]+)", (int.Parse("$1") * JHConfig.Rand.Next(int.Parse("$2"))).ToString());

            await ReplyAsync(result);
        }

        [Command("Roll")]
        [Help("Communication", "Roll a dice!")]
        public async Task RollTheDiceAsync([Remainder]string ToRoll)
        {
            Regex reg = new Regex("([0-9]+)d([0-9]+|\\[([0-9]+-[0-9]+)\\])");
            string ResultString = null;

            foreach (Match roll in reg.Matches(ToRoll))
            {
                int Rollnb = 1;
                int RollValue = 1;
                int MinRoll = 1;
                int Result = 0;

                if (!roll.Success) {
                    await ReplyAsync("You've made an invalid request! Check again");
                    return;
                }

                if (roll.Groups[2].Value.Contains("[", "]")) {
                    string[] Parts = roll.Groups[3].Value.Split("-");
                    MinRoll = int.Parse(Parts[0]);
                    RollValue = int.Parse(Parts[1]);
                    Rollnb = int.Parse(roll.Groups[1].Value);
                } else {
                    Rollnb = int.Parse(roll.Groups[1].Value);
                    RollValue = int.Parse(roll.Groups[2].Value);
                }

                for (int i = 0; i < Rollnb; i++) {
                    Result += JHConfig.Rand.Next(MinRoll, RollValue);
                }
                ToRoll = ToRoll.Replace(roll.Groups[0].Value, Result.ToString());
            }

            DataTable Table = new DataTable();
            try
            {
                ResultString = Table.Compute(ToRoll, "").ToString();
                double result = double.Parse(ResultString);
                ResultString = Convert.ToInt32(result).ToString();

                await ReplyAsync($"You rolled {ResultString}");
            }
            catch (Exception e) when (e is EvaluateException || e is SyntaxErrorException)
            {
                await ReplyAsync("Can't evaluate this roll");
            }
        }

        [Command("Command Info")]
        [Help("Communication", "Get the informations about a command")]
        [Parameter("Command name", "The command you wish to know more about", ParameterType.Mandatory)]
        public async Task DisplayCommandInfosAsync([Remainder]string CommandName)
        {
            var Methods = GetAvailablesCommands();

            EmbedBuilder EmbedMessage = null;

            try
            {
                EmbedMessage = GetCommandsInfosAsync(Methods, CommandName);
            }
            catch (Exception e) when (e is ArgumentException)
            {
                await ReplyAsync("The command name is invalid, please make sure you haven't misspelled it, or execute Help for a list of commands");
                return;
            }

            await ReplyAsync("", false, EmbedMessage.Build());
        }

        public static Embed GetHelperEmbed(string Command)
        {
            CommunicationModule Module = new CommunicationModule();

            var Methods = Module.GetAvailablesCommands();

            return Module.GetCommandsInfosAsync(Methods, Command).Build();

        }

        private EmbedBuilder GetCommandsInfosAsync(MethodInfo[] Commands, string CommandName)
        {
            MethodInfo Command = null;
            HelpAttribute Helper = null;
            List<ParameterAttribute> Parameters;
            List<EmbedFieldBuilder> Fields = new List<EmbedFieldBuilder>();
            

            foreach (MethodInfo info in Commands)
            {
                if (info.GetCustomAttribute<CommandAttribute>().Text.ToUpperInvariant() == CommandName.ToUpperInvariant()) {
                    Command = info;
                    break;
                }
                else if (GetAliases(info) != null && GetAliases(info).ToUpperInvariant().Contains(CommandName.ToUpperInvariant())) {
                    Command = info;
                    break;
                }
            }

            if (Command == null)
                throw new ArgumentException(CommandName);

            CommandName = Command.GetCustomAttribute<CommandAttribute>().Text;
            Helper = Command.GetCustomAttribute<HelpAttribute>();
            Parameters = Command.GetCustomAttributes<ParameterAttribute>().ToList();

            foreach (ParameterAttribute Param in Parameters)
                MakeParameterField(ref Fields, Param);

            Color EmbedColor = new Color(214, 201, 45);

            if (Helper.Warnings != Warnings.None) {
                MakeWarningField(ref Fields, Helper.Warnings);
                EmbedColor = Color.DarkOrange;
            }

            return new EmbedBuilder
            {
                Title = $"{Helper.Category} - {CommandName} {GetAliases(Command)}",
                Description = Helper.Description,
                Color = EmbedColor,
                Fields = Fields
            };
        }

        private void MakeParameterField(ref List<EmbedFieldBuilder> Fields, ParameterAttribute Parameter)
        {
            if (Parameter.Type == ParameterType.Mandatory)
            {
                if (Fields.Any(x => x.Name == "Mandatory Parameters"))
                {
                    Fields.Where(x => x.Name == "Mandatory Parameters").First()
                        .Value += $"{Parameter.Name} - {Parameter.Description} {Environment.NewLine}";
                }
                else
                {
                    Fields.Add(new EmbedFieldBuilder
                    {
                        Name = "Mandatory Parameters",
                        Value = $"{Parameter.Name} - {Parameter.Description} {Environment.NewLine}",
                        IsInline = true
                    });
                }
            }
            else if (Parameter.Type == ParameterType.Optional)
            {
                if (Fields.Any(x => x.Name == "Optional Parameters"))
                {
                    Fields.Where(x => x.Name == "Optional Parameters").First()
                        .Value += $"{Parameter.Name} - {Parameter.Description} {Environment.NewLine}";
                }
                else
                {
                    Fields.Add(new EmbedFieldBuilder
                    {
                        Name = "Optional Parameters",
                        Value = $"{Parameter.Name} - {Parameter.Description} {Environment.NewLine}",
                        IsInline = true
                    });
                }
            }
            else
                return;
        }

        private void MakeWarningField(ref List<EmbedFieldBuilder> Fields, Warnings Warning)
        {
            EmbedFieldBuilder WarningField = new EmbedFieldBuilder
            {
                Name = "Warnings",
                Value = "...",
                IsInline = false
            };

            if ((Warning & Warnings.NSFW) == Warnings.NSFW) {
                WarningField.Value += $"- May contain NSFW content {Environment.NewLine}";
            }
            if ((Warning & Warnings.Spoilers) == Warnings.Spoilers) {
                WarningField.Value += $"- May contain spoilers {Environment.NewLine}";
            }
            if ((Warning & Warnings.RequireAuthorization) == Warnings.RequireAuthorization) {
                WarningField.Value += $"- May need special authorizations (cf. Webhooks)";
            }

            WarningField.Value = WarningField.Value.ToString().Replace("...", "");
            Fields.Add(WarningField);
        }

        private string GetAliases(MethodInfo Command)
        {
            AliasAttribute Alias = Command.GetCustomAttribute<AliasAttribute>();
            if (Alias == null)
                return null;

            string Aliases = " / ";
            foreach (string Ali in Alias.Aliases) {
                Aliases += Ali;
            }

            return Aliases;
        }

        private List<EmbedFieldBuilder> GetCommandsName(MethodInfo[] Commands)
        {
            List<EmbedFieldBuilder> Fields = new List<EmbedFieldBuilder>();

            foreach (MethodInfo info in Commands)
            {
                HelpAttribute CurrentHelper = info.GetCustomAttribute<HelpAttribute>();
                string CommandName = info.GetCustomAttribute<CommandAttribute>().Text;

                if (!Fields.Any(x => x.Name == CurrentHelper.Category))
                    Fields.Add(new EmbedFieldBuilder
                    {
                        Value = $"{CommandName} {GetAliases(info)} {Environment.NewLine}",
                        IsInline = false
                    });
                else
                    Fields.Where(x => x.Name == CurrentHelper.Category).First().Value += $"{CommandName} {GetAliases(info)} {Environment.NewLine}";
            }

            foreach (EmbedFieldBuilder Field in Fields)
                Field.Name = $"**{Field.Name}**";

            return Fields;
        }

        private MethodInfo[] GetAvailablesCommands()
        {
            Assembly CurrentAssembly = Assembly.GetExecutingAssembly();
            var Methods = CurrentAssembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0)
                .Where(m => m.GetCustomAttributes(typeof(HelpAttribute), false).Length > 0)
                .ToArray();

            return Methods;
        }

        public async Task ReRollTextAsync(IUserMessage Message, string Sentence)
        {
            var Response = await GeneratorMinion.CompleteAsync(Sentence, Message, MessageUpdaterAsync).ConfigureAwait(false);

            switch (Response.Error)
            {
                case Error.Complete.Help:
                    await ReplyAsync("No way dat's possible!");
                    break;

                case Error.Complete.Connection:
                    await ReplyAsync("Can't connect to textsynth");
                    break;

                case Error.Complete.None:
                    await Message.ModifyAsync(x => x.Embed = CreateTextEmbed(Response.Answer.Content));
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        public async Task ContinueTextAsync(IUserMessage Message, string Sentence)
        {
            EmbedBuilder emb = Message.Embeds.First().ToEmbedBuilder();
            emb.Footer.Text = "This feature isn't implemented yet...";

            await Message.ModifyAsync(x => x.Embed = emb.Build());
        }

        private async Task BuildContinuedAsync(IUserMessage msg, string Content)
        {
            EmbedBuilder TheEmbed = msg.Embeds.First().ToEmbedBuilder();
            TheEmbed.Fields.Last().Value = Content;

            await msg.ModifyAsync(x => x.Embed = TheEmbed.Build());
        }

        private Embed BuildDefinition(Response.UrbanDefinition InfosBuilder)
        {
            string TransformDefinition = InfosBuilder.Definition.Replace("[", "").Replace("]", "");
            string TransformExemple = InfosBuilder.Exemples.Replace("[", "").Replace("]", "");

            EmbedBuilder builder = new EmbedBuilder
            {
                Color = Color.Blue,
                Url = InfosBuilder.Link,
                Title = InfosBuilder.Word,
                Description = TransformDefinition,
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                       Name = "Exemples",
                       Value = TransformExemple
                    },
                }

            };

            builder.Footer = new EmbedFooterBuilder
            {
                Text = $"Definition by {InfosBuilder.Author}",
            };

            return builder.Build();
        }

        private async Task<string> PureImageAsync(Uri Url)
        {
            Utilities.CheckDir("Ressources/Inspiration");
            string FileName = Url.AbsoluteUri.Split('/')[Url.AbsoluteUri.Split('/').Length - 1];
            string Path = $"Ressources/Inspiration/{FileName}";


            using (WebClient wc = new WebClient())
            {
                await wc.DownloadFileTaskAsync(Url, Path);
            }

            return Path;
        }

        private Embed BuildInspirobotEmbed(Uri Url)
        {
            EmbedBuilder builder = new EmbedBuilder
            {
                ImageUrl = Url.AbsoluteUri,
                Color = Color.DarkBlue
            };

            builder.Footer = new EmbedFooterBuilder
            {
                Text = "Powered by https://inspirobot.me"
            };

            return builder.Build();
        }

        private async Task<IUserMessage> StartWaitAsync(string sentence)
        {
            return await ReplyAsync("", false, new EmbedBuilder
            {
                Color = Color.DarkBlue,
                Description = sentence,
                Footer = new EmbedFooterBuilder
                {
                    Text = "Please wait for embed updating"
                },
            }.Build());
        }
        
        private async Task<Embed> StartContinueAsync(IUserMessage Message)
        {
            if ((Message.Embeds.First() as Embed).Fields.Length == 0) {
                return await InitializeContinueAsync(Message).ConfigureAwait(false);
            }

            EmbedBuilder OldBuilder = Message.Embeds.First().ToEmbedBuilder();
            int IterationNumber = OldBuilder.Fields.Count + 1;

            OldBuilder.AddField(new EmbedFieldBuilder
            {
                Name = $"Iteration {IterationNumber}",
                Value = "[Generating...]",
                IsInline = false
            });
            OldBuilder.Title = $"Iteration {IterationNumber}";
            OldBuilder.Footer.Text = "Waiting for continuation...";

            await Message.ModifyAsync(x => x.Embed = OldBuilder.Build());
            return Message.Embeds.First() as Embed;
        }

        private async Task<Embed> InitializeContinueAsync(IUserMessage Message)
        {
            string FirstIteration = Message.Embeds.First().Description;

            await Message.ModifyAsync(x => x.Embed = new EmbedBuilder
            {
                Title = "Iteration 2",
                Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        Name = "Iteration 1",
                        Value = FirstIteration,
                        IsInline = false
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Iteration 2",
                        Value = "[Generating...]",
                        IsInline = false
                    }
                },
                Footer = new EmbedFooterBuilder
                {
                   Text = "Continuing text..."
                },
                Color = Color.DarkBlue
            }.Build());

            return Message.Embeds.First() as Embed;
        }
        
        private Embed CreateIterationEmbed(Embed BaseEmbed)
        {
            int CurrentLoop = BaseEmbed.Fields.Length + 1;

            EmbedBuilder ContinuedBuilder = new EmbedBuilder
            {
                Color = Color.DarkBlue,
                Footer = new EmbedFooterBuilder
                {
                    Text = "Continuing the text..."
                },
            };

            if (CurrentLoop > 1)
            {
                foreach (var Field in BaseEmbed.Fields)
                {
                    ContinuedBuilder.Fields.Add(new EmbedFieldBuilder { Name = Field.Name, Value = Field.Value, IsInline = Field.Inline });
                }
            }
            else
            {
                ContinuedBuilder.AddField(new EmbedFieldBuilder { Name = "Iteration 1", Value = BaseEmbed.Description, IsInline = false });
                CurrentLoop++;
            }
            if (CurrentLoop > MaxIterationNumber)
            {
                ContinuedBuilder.Footer = new EmbedFooterBuilder { Text = "You've reached the maximum number of iterations" };
                return ContinuedBuilder.Build();
            }
            ContinuedBuilder.AddField(new EmbedFieldBuilder { Name = $"Iteration {CurrentLoop}", Value = "[generating...]", IsInline = false });
            ContinuedBuilder.Title = "Iteration " + ContinuedBuilder.Fields.Count;
            return ContinuedBuilder.Build();
        }

        private Embed CreateTextEmbed(string Content)
        {
            Content = GenerationMessageCleaner(Content);

            return new EmbedBuilder
            {
                Color = Color.DarkBlue,
                Description = Content,
                Footer = new EmbedFooterBuilder
                {
                    Text = "Powered by https://bellard.org/textsynth/"
                },
            }.Build();
        }

        public async Task IterationUpdater(IUserMessage msg, string Content)
        {
            Content = GenerationMessageCleaner(Content);
            EmbedBuilder Ember = msg.Embeds.First().ToEmbedBuilder();
            string MessageEmbeds = GetLastMessage(msg.Embeds.First() as Embed);
            MessageEmbeds.Replace("[Generating...]", "");

            if (MessageEmbeds == Content)
                return;

            string UsableContent = Content.Split(MessageEmbeds, StringSplitOptions.None)[1];
            Ember.Fields.Last().Value = UsableContent;
            await msg.ModifyAsync(x => x.Embed = Ember.Build());
        }

        private string GetLastMessage(Embed embed)
        {
            string EndMessage = null;

            foreach (var Field in embed.Fields)
            {
                EndMessage += Field.Value;
            }

            return EndMessage;
        }

        public async Task MessageUpdaterAsync(IUserMessage msg, string Content)
        {
            Content = GenerationMessageCleaner(Content);

            string url = (msg.Embeds.ElementAt(0) as Embed).Url;
            await msg.ModifyAsync(x => x.Embed = new EmbedBuilder
            {
                Description = Content,
                Color = Color.DarkBlue,
                Footer = new EmbedFooterBuilder
                {
                    Text = "Please wait for embed updating"
                },
                Url = url
            }.Build());
        }

        private string GenerationMessageCleaner(string msg)
        {
            return msg.Replace(" .", ".", StringComparison.OrdinalIgnoreCase)
                .Replace("\" ", "\"", StringComparison.OrdinalIgnoreCase).
                Replace("' ", "'", StringComparison.OrdinalIgnoreCase).
                Replace(" '", "'", StringComparison.OrdinalIgnoreCase).
                Replace(" ,", ",", StringComparison.OrdinalIgnoreCase).
                Replace("( ", "(", StringComparison.OrdinalIgnoreCase).
                Replace(" )", ")", StringComparison.OrdinalIgnoreCase);
        }
    }
}
