using Discord;
using Discord.Commands;
using J.H_D.Minions.Infos;
using J.H_D.Tools;
using RethinkDb.Driver.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Modules
{
    class MovieModule : ModuleBase
    {
        Program p = Program.p;

        [Command("Get movie")]
        public async Task GetMovie(params string[] Args)
        {
            // Availlability check
            await p.DoAction(Context.User, Context.Guild.Id, Program.Module.Movie);
            // Owner only settings
            var result = await Minions.Infos.MovieMinion.SearchMovie(Minions.Infos.MovieMinion.SearchType.Movie, Args, null);
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

        private Embed CreateEmbedSimple(Response.Movie res, ulong guildId)
        {
            string Name = res.Name;
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = Name,
                Url = "https://www.themoviedb.org/movie/" + res.Id + "-" + res.OriginalTitle.Replace(' ', '-'),
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
