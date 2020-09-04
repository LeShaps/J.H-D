using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

using J.H_D.Tools;
using J.H_D.Data;
using J.H_D.Minions.Websites;
using J.H_D.Minions.Infos;
using System;

namespace J.H_D.Modules
{
    class CommunicationModule : ModuleBase
    {
        [Command("Hey !")]
        public async Task ReplytoAsync()
        {
            await ReplyAsync("Hi !");
        }

        [Command("Rotate")]
        public async Task RotateThenRespondAsync([Remainder]string Args)
        {
            await ReplyAsync(Utilities.RotateString(Args));
        }

        [Command("Clarify")]
        public async Task ClarifyThenRespondAsync(params string[] Args)
        {
            await ReplyAsync(Utilities.GetPlainTextFromHtml(Utilities.MakeArgs(Args)));
        }

        [Command("Motive"), Alias("Inspire")]
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

        [Command("Define")]
        public async Task FindDefinitionAsync(params string[] Args)
        {
            string FinalArgs = Utilities.MakeQueryArgs(Args);

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
        public async Task GenerateAsync([Remainder]string sentence)
        {
            var msg = await StartWaitAsync(sentence).ConfigureAwait(false);
            var Response = await GeneratorMinion.CompleteAsync(sentence, msg, MessageUpdaterAsync);

            switch (Response.Error)
            {
                case Error.Complete.Help:
                    await ReplyAsync("Please enter a phrase to use as base");
                    break;

                case Error.Complete.Connection:
                    await ReplyAsync("Can't connect to textsynth");
                    break;

                case Error.Complete.None:
                    await msg.ModifyAsync(x => x.Embed = CreateTextEmbed(Response.Answer.Content));
                    JHConfig.GeneratedText.Add(msg.Id, sentence);
                    await msg.AddReactionAsync(new Emoji("🔄"));
                    break;

                default:
                    throw new NotSupportedException();
            }
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

        public async Task ContinueTextAsync(IUserMessage Message)
        {
            string MessageSentence = Message.Content;
            await ReplyAsync(MessageSentence);
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

        private Embed CreateTextEmbed(string Content)
        {
            Content = Content.Replace(" .", ".", StringComparison.OrdinalIgnoreCase)
                .Replace("\" ", "\"", StringComparison.OrdinalIgnoreCase).
                Replace("' ", "'", StringComparison.OrdinalIgnoreCase).
                Replace(" '", "'", StringComparison.OrdinalIgnoreCase).
                Replace(" ,", ",", StringComparison.OrdinalIgnoreCase).
                Replace("( ", "(", StringComparison.OrdinalIgnoreCase).
                Replace(" )", ")", StringComparison.OrdinalIgnoreCase);

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

        public async Task MessageUpdaterAsync(IUserMessage msg, string Content)
        {
            Content = Content.Replace(" .", ".", StringComparison.OrdinalIgnoreCase)
                .Replace("\" ", "\"", StringComparison.OrdinalIgnoreCase).
                Replace("' ", "'", StringComparison.OrdinalIgnoreCase).
                Replace(" '", "'", StringComparison.OrdinalIgnoreCase).
                Replace(" ,", ",", StringComparison.OrdinalIgnoreCase).
                Replace("( ", "(", StringComparison.OrdinalIgnoreCase).
                Replace(" )", ")", StringComparison.OrdinalIgnoreCase);

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
    }
}
