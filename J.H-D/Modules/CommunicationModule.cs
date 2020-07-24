using Discord;
using Discord.Commands;
using J.H_D.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using J.H_D.Minions;
using System.Net.WebSockets;
using J.H_D.Minions.Responses;

namespace J.H_D.Modules
{
    class CommunicationModule : ModuleBase
    {
        [Command("Hey !")]
        public async Task ReplytoAsync()
        {
            if (Context.User.Id == 151425222304595968)
                await ReplyAsync("No, not you");
            else
                await ReplyAsync("Hi !");
        }

        [Command("Rotate")]
        public async Task RotateThenRespond(params string[] Args)
        {
            string Ask = Utilities.MakeArgs(Args);
            await ReplyAsync(Utilities.RotateString(Ask));
        }

        [Command("Clarify")]
        public async Task ClarifyThenRespond(params string[] Args)
        {
            await ReplyAsync(Utilities.GetPlainTextFromHtml(Utilities.MakeArgs(Args)));
        }

        [Command("Motive"), Alias("Inspire")]
        public async Task Motive(params string[] Args)
        {
            string cleanArgs = Utilities.MakeArgs(Args).ToLower();
            bool Natural = false;


            if (cleanArgs == "clean") Natural = true;

            var Result = await Minions.Responses.InspirobotMinion.FeelInspiration();

            switch (Result.Error)
            {
                case Error.InspirationnalError.Communication:
                    await ReplyAsync("I'm not feeling too inspired today");
                    break;

                case Error.InspirationnalError.None:
                    if (!Natural)
                        await ReplyAsync("", false, BuildInspirobotEmbed(Result.Answer));
                    else
                    {
                        string file = await PureImage(Result.Answer);
                        await Context.Channel.SendFileAsync(file);
                        File.Delete(file);
                    }
                    break;
            }
        }

        [Command("Define")]
        public async Task FindDefinition(params string[] Args)
        {
            string FinalArgs = Utilities.MakeQueryArgs(Args);

            var Result = await UrbanDictionaryMinion.SearchForWord(FinalArgs);

            switch (Result.Error)
            {
                case Minions.Responses.Error.Urban.WordNotFound:
                    await ReplyAsync("Sorry, even internet can't help you on this one");
                    break;

                case Minions.Responses.Error.Urban.None:
                    await ReplyAsync("", false, BuildDefinition(Result.Answer));
                    break;
            }
        }

        private Embed BuildDefinition(Minions.Responses.Response.UrbanDefinition InfosBuilder)
        {
            string TransformDefinition = InfosBuilder.Definition.Replace("[", "").Replace("]", "");
            string TransformExemple = InfosBuilder.Exemples.Replace("[", "").Replace("]", "");

            EmbedBuilder builder = new EmbedBuilder()
            {
                Color = Color.Blue,
                Url = InfosBuilder.Link,
                Title = InfosBuilder.Word,
                Description = TransformDefinition,
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                       Name = "Exemples",
                       Value = TransformExemple
                    },
                }

            };

            builder.Footer = new EmbedFooterBuilder()
            {
                Text = $"Definition by {InfosBuilder.Author}",
            };

            return builder.Build();
        }

        private async Task<string> PureImage(string Url)
        {
            Utilities.CheckDir("Ressources/Inspiration");
            string FileName = Url.Split('/')[Url.Split('/').Length - 1];
            string Path = $"Ressources/Inspiration/{FileName}";


            using (WebClient wc = new WebClient())
            {
                await wc.DownloadFileTaskAsync(Url, Path);
            }

            return Path;
        }

        private Embed BuildInspirobotEmbed(string Url)
        {
            EmbedBuilder builder = new EmbedBuilder()
            {
                ImageUrl = Url,
                Color = Color.DarkBlue
            };

            builder.Footer = new EmbedFooterBuilder()
            {
                Text = "Powered by https://inspirobot.me"
            };

            return builder.Build();
        }
    }
}
