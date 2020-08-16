using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

using J.H_D.Minions.Infos;
using J.H_D.Tools;
using J.H_D.Data;

namespace J.H_D.Modules
{
    class MusicModule : ModuleBase
    {
        [Command("Get music artist")]
        public async Task GetArtistInfos(params string[] Args)
        {
            string Artist = Utilities.MakeQueryArgs(Args);

            var Result = await MusicMinion.LookForArtistAsync(Artist);

            switch (Result.Error)
            {
                case Error.Brainz.ArtistNotFound:
                    await ReplyAsync("That must be too underground for me, sorry");
                    break;

                case Error.Brainz.TitleNotFound:
                    await ReplyAsync("Hey, the refrain isn't always the title, y'know?");
                    break;

                case Error.Brainz.ConnectionError:
                    await ReplyAsync("A connection error has happened, please retry later");
                    break;

                case Error.Brainz.None:
                    await ReplyAsync("", false, MakeArtistEmbed((Response.MusicArtist)Result.Answer));
                    break;
            }
        }

        private Embed MakeArtistEmbed(Response.MusicArtist Artist)
        {
            string NoPromoDescription = Artist.Bio.Substring(0, Artist.Bio.IndexOf("<a"));
            NoPromoDescription = Utilities.DiscordFriendly(NoPromoDescription);
            EmbedBuilder builder = new EmbedBuilder()
            {
                Title = Artist.Name,
                Url = Artist.LastUrl,
                Description = NoPromoDescription,
            };

            builder.AddField(new EmbedFieldBuilder()
            {
                Name = "Genres",
                Value = String.Join(Environment.NewLine, Artist.Genres.ToArray()),
                IsInline = true
            });

            builder.AddField(new EmbedFieldBuilder()
            {
                Name = "On Tour",
                Value = Artist.OnTour ? "Yes" : "Nope",
                IsInline = true
            });

            return builder.Build();
        }
    }
}
