using Discord;
using Discord.Commands;
using J.H_D.Minions.Infos;
using J.H_D.Tools;
using RethinkDb.Driver.Ast;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Modules
{
    class MovieModule : ModuleBase
    {
        Program p = Program.p;

        [Command("Get movie"), Priority(-1)]
        public async Task GetMovie(params string[] Args)
        {
            // Availlability check
            await p.DoAction(Context.User, Context.Guild.Id, Program.Module.Movie);
            // Owner only settings
            var result = await Minions.Infos.MovieMinion.SearchMovie(Args);
            switch (result.Error)
            {
                case Minions.Infos.Error.Movie.Help:
                    await ReplyAsync("That's not how that work");
                    break;

                case Minions.Infos.Error.Movie.NotFound:
                    await ReplyAsync("I don't know what you're looking for, but it's definitively not here");
                    break;

                case Minions.Infos.Error.Movie.None:
                    await ReplyAsync("", false, CreateEmbedSimple(result.Answer, Context.Guild.Id));
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        [Command("Get movie infos"), Alias("Get movie info")]
        public async Task GetMovieInfos(params string[] Args)
        {
            await p.DoAction(Context.User, Context.Guild.Id, Program.Module.Movie);

            var result = await Minions.Infos.MovieMinion.BonusInfos(MovieMinion.SearchType.Serie, Args);

            switch (result.Error)
            {
                case Minions.Infos.Error.Movie.Help:
                    await ReplyAsync("That's not how that work");
                    break;

                case Minions.Infos.Error.Movie.NotFound:
                    await ReplyAsync("I don't know what you're looking for, but it's definitively not here");
                    break;

                case Minions.Infos.Error.Movie.None:
                    await ReplyAsync("", false, CreateBonusEmbed(result.Answer));
                    break;
            }
        }

        private Embed CreateBonusEmbed(Response.Movie res)
        {
            string CorrectedUrl = res.OriginalTitle.Replace(' ', '-').Replace('\'', '-');
            string Budget = res.Budget.ToString("N0", new NumberFormatInfo()
            {
                NumberGroupSizes = new[] {3},
                NumberGroupSeparator = " "
            });

            string Revenue = res.Revenue.ToString("N0", new NumberFormatInfo()
            {
                NumberGroupSizes = new[] { 3 },
                NumberGroupSeparator = " "
            });

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"More infos about {res.Name.Replace(":", "h")}",
                Url = $"https://themoviedb.org/movie/{res.Id}-{CorrectedUrl}",
                Color = Color.DarkRed,
                ImageUrl = $"{res.RessourcePath}{res.BackdropPath}",
                Description = $"Runtime : {res.Runtime}"
            };

            embed.AddField("Production Companies", String.Join(Environment.NewLine, res.ProductionCompanies.ToArray()), true);
            embed.AddField("Genres", String.Join(Environment.NewLine, res.Genres.ToArray()), true);

            embed.AddField("Budget", $"{Budget}$", false);
            embed.AddField("Revenue", $"{Revenue}$", true);

            embed.Footer = new EmbedFooterBuilder()
            {
               Text = $"Average note : {res.AverageNote}",
               IconUrl = "https://www.themoviedb.org/assets/2/v4/logos/v2/blue_square_2-d537fb228cf3ded904ef09b136fe3fec72548ebc1fea3fbbd1ad9e36364db38b.svg"
            };

            return embed.Build();
        }

        private Embed CreateEmbedSimple(Response.Movie res, ulong guildId)
        {
            string CorrectedUrl = res.OriginalTitle.Replace(' ', '-').Replace('\'', '-');
            EmbedBuilder embed = new EmbedBuilder()
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
    }
}
