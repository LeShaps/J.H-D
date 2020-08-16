using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using J.H_D.Data;
using J.H_D.Minions;
using J.H_D.Minions.NSFW;
using J.H_D.Tools;

namespace J.H_D.Modules
{
    class BooruModule : ModuleBase
    {
        [Command("Konachan", RunMode = RunMode.Async), Priority(-1)]
        public async Task SearchKonachan(params string[] Args)
        {
            await Program.p.DoAction(Context.User, Context.Guild.Id, Program.Module.Booru);

            var result = await BooruMinion.GetBooruImageAsync(new BooruMinion.BooruOptions(BooruMinion.BooruType.Konachan, Args, Utilities.IsChannelNSFW(Context)));

            await ProccessResult(result);
        }

        [Command("Konachan with infos", RunMode = RunMode.Async)]
        public async Task SearchWithBonus(params string[] Args)
        {
            await Program.p.DoAction(Context.User, Context.Guild.Id, Program.Module.Booru);

            var result = await BooruMinion.GetBooruImageAsync(new BooruMinion.BooruOptions(BooruMinion.BooruType.Konachan, Args, Utilities.IsChannelNSFW(Context)));

            await ProccessInfosResultAsync(result, BooruMinion.BooruType.Konachan);
        }

        private async Task ProccessResult(FeatureRequest<BooruSharp.Search.Post.SearchResult, Error.Booru> Result)
        {
            if (!Utilities.IsChannelNSFW(Context) && Result.Answer.rating != BooruSharp.Search.Post.Rating.Safe)
            {
                await ReplyAsync("No safe image was found with theses parameters, please try on an NSFW channel or with others");
                return;
            }

            switch (Result.Error)
            {
                case Error.Booru.NotFound:
                    await ReplyAsync("I don't know what you're looking for, but it's definitively not here!");
                    break;

                case Error.Booru.None:
                    await ReplyAsync("", false, BuildImageEmbed(Result.Answer)).ConfigureAwait(false);
                    break;

                default:
                    break;
            }
        }

        private async Task ProccessInfosResultAsync(FeatureRequest<BooruSharp.Search.Post.SearchResult, Error.Booru> Result, BooruMinion.BooruType Website)
        {
            if (!Utilities.IsChannelNSFW(Context) && Result.Answer.rating != BooruSharp.Search.Post.Rating.Safe)
            {
                await ReplyAsync("No safe image was found with theses parameters, please try on an NSFW channel or with others");
                return;
            }

            switch (Result.Error)
            {
                case Error.Booru.NotFound:
                    await ReplyAsync("I don't know what you're looking for, but it's definitively not here!");
                    break;

                case Error.Booru.None:
                    await ReplyAsync("", false, await BuildImageInfosEmbedAsync(Result.Answer, Website));
                    break;

                default:
                    break;
            }
        }

        private Embed BuildImageEmbed(BooruSharp.Search.Post.SearchResult Result)
        {
            EmbedBuilder emb = new EmbedBuilder()
            {
                Title = "Sauce",
                Url = Result.source,
                ImageUrl = Result.fileUrl.AbsoluteUri,
            };

            switch (Result.rating)
            {
                case BooruSharp.Search.Post.Rating.Safe:
                    emb.Color = Color.Green;
                    break;

                case BooruSharp.Search.Post.Rating.Questionable:
                    emb.Color = Color.LightOrange;
                    break;

                case BooruSharp.Search.Post.Rating.Explicit:
                    emb.Color = Color.Purple;
                    break;
            }
            
            emb.Footer = new EmbedFooterBuilder()
            {
                Text = $"Posted the {Result.creation}"
            };
            return emb.Build();
        }

        private async Task<Embed> BuildImageInfosEmbedAsync(BooruSharp.Search.Post.SearchResult Result, BooruMinion.BooruType Website)
        {
            EmbedBuilder emb = new EmbedBuilder()
            {
                Title = "Sauce",
                Url = Result.source,
                ImageUrl = Result.fileUrl.AbsoluteUri,
            };

            switch (Result.rating)
            {
                case BooruSharp.Search.Post.Rating.Safe:
                    emb.Color = Color.Blue;
                    break;

                case BooruSharp.Search.Post.Rating.Questionable:
                    emb.Color = Color.LightOrange;
                    break;

                case BooruSharp.Search.Post.Rating.Explicit:
                    emb.Color = Color.Purple;
                    break;

                default:
                    break;
            }

            var TagResults = await BooruMinion.GetTags(Website, Result.tags);
            List<BooruSharp.Search.Tag.SearchResult> FoundTags = TagResults.Answer;

            string Artist = null;
            string Parodies = null;
            string GeneralTags = null;
            string Characters = null;

            foreach (var Tag in FoundTags.Where(x => x.type == BooruSharp.Search.Tag.TagType.Artist))
                { Artist += $"{CleanTag(Tag.name)}{Environment.NewLine}"; }
            foreach (var Tag in FoundTags.Where(x => x.type == BooruSharp.Search.Tag.TagType.Copyright))
                { Parodies += $"{CleanTag(Tag.name)}{Environment.NewLine}"; }
            foreach (var Tag in FoundTags.Where(x => x.type == BooruSharp.Search.Tag.TagType.Character))
                { Characters += $"{CleanTag(Tag.name, true)}{Environment.NewLine}"; }
            foreach (var Tag in FoundTags.Where(x => x.type == BooruSharp.Search.Tag.TagType.Trivia))
                { GeneralTags += $"{CleanTag(Tag.name)}{Environment.NewLine}"; }

            emb.AddField(new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Artist",
                Value = Artist == null ? "Not found" : Artist
            });

            emb.AddField(new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Parodies",
                Value = Parodies == null ? "Original" : Parodies
            });

            emb.AddField(new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Characters",
                Value = Characters == null ? "Original" : Characters
            });

            emb.AddField(new EmbedFieldBuilder()
            {
                IsInline = true,
                Name = "Tags",
                Value = GeneralTags == null ? "" : GeneralTags
            });

            emb.Footer = new EmbedFooterBuilder()
            {
                Text = $"Posted the {Result.creation}"
            };
            return emb.Build();
        }

        private string CleanTag(string tag, bool name = false)
        {
            tag = tag.Replace('_', ' ');
            tag = char.ToUpper(tag[0]) + tag.Substring(1);

            if (name)
            {
                string[] Name = tag.Split(' ');
                for (int i = 0; i < Name.Length; i++)
                    Name[i] = char.ToUpper(Name[i][0]) + Name[i].Substring(1);

                tag = String.Join(" ", Name);
            }

            return tag;
        }
    }
}
