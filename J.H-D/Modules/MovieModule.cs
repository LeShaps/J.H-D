using Discord;
using Discord.Commands;
using System;
using System.Globalization;
using System.Threading.Tasks;

using J.H_D.Minions.Infos;
using J.H_D.Tools;
using J.H_D.Data;
using System.Linq;

namespace J.H_D.Modules
{
    class MovieModule : ModuleBase
    {
        [Command("Get movie"), Priority(-1)]
        [Help("Movie", "Get infos about a movie", Warnings.RequireAuthorization)]
        [Parameter("Movie", "The name of the movie", ParameterType.Mandatory)]
        public async Task GetMovieAsync(params string[] Args)
        {
            if (Context.Channel as ITextChannel is null)
                await Program.DoActionAsync(Context.User, 0, Module.Movie);
            else
                await Program.DoActionAsync(Context.User, Context.Channel.Id, Module.Movie);

            var result = await MovieMinion.SearchMovieAsync(Args);
            switch (result.Error)
            {
                case Error.Movie.Help:
                    await ReplyAsync("", false, CommunicationModule.GetHelperEmbed("Get movie"));
                    break;

                case Error.Movie.NotFound:
                    await ReplyAsync("I don't know what you're looking for, but it's definitively not here");
                    break;

                case Error.Movie.None:
                    await ReplyAsync("", false, CreateEmbedSimple(result.Answer));
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        [Command("Get series")]
        [Help("Movie", "Get infos about a serie")]
        [Parameter("Serie", "The serie of the serie", ParameterType.Mandatory)]
        public async Task GetSeriesInfosAsync(params string[] Args)
        {
            var result = await MovieMinion.GetSeriesGeneralInfosAsync(MovieMinion.SearchType.Serie, Args);

            switch (result.Error)
            {
                case Error.Movie.Help:
                    await ReplyAsync("", false, CommunicationModule.GetHelperEmbed("Get series"));
                    break;

                case Error.Movie.NotFound:
                    await ReplyAsync("I don't know what you're looking for, but it's definitively not here");
                    break;

                case Error.Movie.None:
                    var Message = await ReplyAsync("", false, CreateSeriesEmbed(result.Answer));
                    JHConfig.SendedSeriesEmbed.Add(Message.Id, new Tuple<int, Response.TVSeries>(-1, result.Answer));
                    await Message.AddReactionsAsync(new[] { new Emoji("⏪"), new Emoji("◀️"), new Emoji("▶️"), new Emoji("⏩") });
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        [Command("Get movie infos"), Alias("Get movie info")]
        [Help("Movie", "Gather more infos about the movie, it's box office, the productions companies, ect...", Warnings.RequireAuthorization | Warnings.Spoilers)]
        [Parameter("Name", "The name of the movie", ParameterType.Mandatory)]
        public async Task GetMovieInfosAsync(params string[] Args)
        {
            await Program.DoActionAsync(Context.User, Context.Guild.Id, Module.Movie);

            var result = await MovieMinion.BonusInfosAsync(MovieMinion.SearchType.Serie, Args);

            switch (result.Error)
            {
                case Error.Movie.Help:
                    await ReplyAsync("", false, CommunicationModule.GetHelperEmbed("Get movie infos"));
                    break;

                case Error.Movie.NotFound:
                    await ReplyAsync("I don't know what you're looking for, but it's definitively not here");
                    break;

                case Error.Movie.None:
                    await ReplyAsync("", false, CreateBonusEmbed(result.Answer));
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private Embed CreateBonusEmbed(Response.Movie res)
        {
            string CorrectedUrl = res.OriginalTitle.Replace(' ', '-').Replace('\'', '-');
            string Budget = res.Budget.ToString("N0", new NumberFormatInfo
            {
                NumberGroupSizes = new[] {3},
                NumberGroupSeparator = " "
            });

            string Revenue = res.Revenue.ToString("N0", new NumberFormatInfo
            {
                NumberGroupSizes = new[] { 3 },
                NumberGroupSeparator = " "
            });

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = $"More infos about {res.Name.Replace(":", "h")}",
                Url = $"https://themoviedb.org/movie/{res.Id}-{CorrectedUrl}",
                Color = Color.DarkRed,
                ImageUrl = $"{res.RessourcePath}{res.BackdropPath}",
                Description = $"Runtime : {res.Runtime}"
            };

            embed.AddField("Production Companies", String.Join(Environment.NewLine, res.ProductionCompanies), true);
            embed.AddField("Genres", String.Join(Environment.NewLine, res.Genres), true);

            embed.AddField("Budget", $"{Budget}$", false);
            embed.AddField("Revenue", $"{Revenue}$", true);

            embed.Footer = new EmbedFooterBuilder
            {
               Text = $"Average note : {res.AverageNote}",
               IconUrl = "https://www.themoviedb.org/assets/2/v4/logos/v2/blue_square_2-d537fb228cf3ded904ef09b136fe3fec72548ebc1fea3fbbd1ad9e36364db38b.svg"
            };

            return embed.Build();
        }

        private Embed CreateSeriesEmbed(Response.TVSeries Response)
        {
            string CorrectedUrl = Response.SeriesName.Replace(' ', '-').Replace('\'', '-');
            EmbedBuilder embed = new EmbedBuilder
            {
                Title = Response.SeriesName,
                Url = $"https://themoviedb.org/tv/{Response.SeriesId}-{CorrectedUrl}",
                Color = Color.DarkRed,
                ImageUrl = $"{Response.RessourcePath}{Response.BackdropPath}",
                Description = Response.Overview
            };
            embed.AddField("Release date", Response.Started, true);
            embed.AddField("Average note", Response.VoteAverage, true);

            return embed.Build();
        }

        private Embed CreateSeasonEmbed(Response.TVSeason Season)
        {
            EmbedBuilder build = new EmbedBuilder
            {
                Title = Season.SName,
                Description = Season.Overview,
                ImageUrl = $"{Season.RessourcePath}{Season.PosterPath}",
                Color = Color.DarkRed,
            };

            build.Fields.Add(new EmbedFieldBuilder
            {
                Name = "Number of Episodes",
                Value = Season.EpisodeNumber,
                IsInline = true
            });
            build.Fields.Add(new EmbedFieldBuilder
            {
                Name = "Season",
                Value = Season.SNumber,
                IsInline = true
            });
            
            return build.Build();
        }

        private Embed CreateEmbedSimple(Response.Movie res)
        {
            string CorrectedUrl = res.OriginalTitle.Replace(' ', '-').Replace('\'', '-');
            EmbedBuilder embed = new EmbedBuilder
            {
                Title = res.Name,
                Url = $"https://www.themoviedb.org/movie/{res.Id}-{CorrectedUrl}",
                Color = Color.DarkRed,
                ImageUrl = res.RessourcePath + res.PosterPath,
                Description = Utilities.DiscordFriendly(res.Overview)
            };
            embed.AddField("Release date", res.ReleaseDate, true);
            embed.AddField("Average note", res.AverageNote + "/10", true);

            return embed.Build();
        }

        public async Task<Tuple<int, Response.TVSeries>> UpdateSeriesEmbedAsync(IUserMessage message, Tuple<int, Response.TVSeries> Data, int NewPosition)
        {
            if (NewPosition < -1 || NewPosition >= int.Parse(Data.Item2.SeasonNumber))
                return Data;

            await message.ModifyAsync(x =>
            x.Embed = NewPosition == -1 ?
            CreateSeriesEmbed(Data.Item2) :
            CreateSeasonEmbed(Data.Item2.Seasons.ElementAt(NewPosition)));

            return new Tuple<int, Response.TVSeries>(NewPosition, Data.Item2);
        }
    }
}
