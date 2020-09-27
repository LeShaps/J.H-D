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
        [Help("Music", "Get infos about a music artist")]
        [Parameter("Artist", "The name of the artist", ParameterType.Mandatory)]
        public async Task GetArtistInfosAsync(params string[] Args)
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
                    
                default:
                    throw new NotSupportedException();
            }
        }

        [Command("Lyrics")]
        [Help("Music", "Find the lyrics of a song")]
        [Parameter("Song name", "The name of the song", ParameterType.Mandatory)]
        [Parameter("Artist name", "Name of the artist, recommended", ParameterType.Optional)]
        public async Task GetSongLyrics(params string[] Args)
        {
            var Results = await MusicMinion.GetLyrics(Args);

            switch (Results.Error)
            {
                case Error.LyricsMatch.NotFound:
                    await ReplyAsync("I couldn't find theses lyrics");
                    break;

                case Error.LyricsMatch.Help:
                    await ReplyAsync("", false, CommunicationModule.GetHelperEmbed("Lyrics"));
                    break;

                case Error.LyricsMatch.None:
                    await ReplyAsync("", false, BuildLyricsEmbed((Response.SongLyrics)Results.Answer));
                    break;
            }
        }

        private Embed MakeArtistEmbed(Response.MusicArtist Artist)
        {
            string NoPromoDescription = Artist.Bio.Substring(0, Artist.Bio.IndexOf("<a"));
            NoPromoDescription = Utilities.DiscordFriendly(NoPromoDescription);
            EmbedBuilder builder = new EmbedBuilder
            {
                Title = Artist.Name,
                Url = Artist.LastUrl.AbsoluteUri,
                Description = NoPromoDescription,
            };

            builder.AddField(new EmbedFieldBuilder
            {
                Name = "Genres",
                Value = String.Join(Environment.NewLine, Artist.Genres),
                IsInline = true
            });

            builder.AddField(new EmbedFieldBuilder
            {
                Name = "On Tour",
                Value = Artist.OnTour ? "Yes" : "Nope",
                IsInline = true
            });

            return builder.Build();
        }

        private Embed BuildLyricsEmbed(Response.SongLyrics Lyrics)
        {
            return new EmbedBuilder
            {
                Title = String.Join(" - ", Lyrics.Artist, Lyrics.SongName),
                Description = Lyrics.Lyrics,
                Color = Color.Green,
                Footer = new EmbedFooterBuilder
                {
                    Text = "Lyrics founds on Google"
                }
            }.Build();
        }
    }
}
