using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using J.H_D.Minions.Infos;
using J.H_D.Data;
using Anime = J.H_D.Data.Response.Anime;
using Discord;
using J.H_D.Tools;

namespace J.H_D.Modules
{
    class AnimeMangaModule : ModuleBase
    {
        [Command("Get anime", RunMode = RunMode.Async)]
        public async Task GetAnime(params string[] Args)
        {
            var Response = await KitsuMinion.SearchAnime(Args);

            switch (Response.Error)
            {
                case Error.Anime.Help:
                    await ReplyAsync("Please enter an anime name");
                    break;

                case Error.Anime.NotFound:
                    await ReplyAsync("Sorry, I can't find this one");
                    break;

                case Error.Anime.None:
                    await ReplyAsync("", false, BuildAnimeEmbed(Response.Answer));
                    break;
            }
        }

        private Embed BuildAnimeEmbed(Anime Result)
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = Color.Blue,
                Title = $"{Result.Title} ({Result.LATitle})",
                Description = $"{Utilities.GetPlainTextFromHtml(Result.Synopsis)}",
                ImageUrl = Result.PosterImage,
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Episodes",
                        Value = Result.EpisodeCount,
                        IsInline = true
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Total watchtime",
                        Value = Result.HumanReadableWatchtime,
                        IsInline = true
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Status",
                        Value = Utilities.StandardUppercase(Result.Status),
                        IsInline = true
                    },
                    new EmbedFieldBuilder()
                    {
                        Name = "Rating",
                        Value = $"{Result.AgeRating} ({Result.Guideline})",
                        IsInline = true
                    }
                },
                Footer = new EmbedFooterBuilder()
                {
                    Text = $"Average note of {Result.Rating} out of 100"
                }
            };

            return embed.Build();
        }
    }
}
