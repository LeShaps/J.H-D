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
using System.Reflection.Emit;
using Microsoft.VisualBasic;

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
                    await msg.AddReactionsAsync(new[] { new Emoji("🔄"), new Emoji("▶️") });
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

        public async Task ContinueTextAsync(IUserMessage Message, string Sentence)
        {
            EmbedBuilder emb = Message.Embeds.First().ToEmbedBuilder();
            emb.Footer.Text = "This feature isn't implemented yet...";

            await Message.ModifyAsync(x => x.Embed = emb.Build());
        }

        private async Task BuildContinued(IUserMessage msg, string Content)
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
                return await InitializeContinueAsync(Message);
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

            EmbedBuilder ContinuedBuilder = new EmbedBuilder()
            {
                Color = Color.DarkBlue,
                Footer = new EmbedFooterBuilder()
                {
                    Text = "Continuing the text..."
                },
            };

            if (CurrentLoop > 1)
            {
                foreach (var Field in BaseEmbed.Fields)
                {
                    ContinuedBuilder.Fields.Add(new EmbedFieldBuilder() { Name = Field.Name, Value = Field.Value, IsInline = Field.Inline });
                }
            }
            else
            {
                ContinuedBuilder.AddField(new EmbedFieldBuilder() { Name = "Iteration 1", Value = BaseEmbed.Description, IsInline = false });
                CurrentLoop++;
            }
            if (CurrentLoop > 10)
            {
                ContinuedBuilder.Footer = new EmbedFooterBuilder() { Text = "You've reached the maximum number of iterations" };
                return ContinuedBuilder.Build();
            }
            ContinuedBuilder.AddField(new EmbedFieldBuilder() { Name = $"Iteration {CurrentLoop}", Value = "[generating...]", IsInline = false });
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
