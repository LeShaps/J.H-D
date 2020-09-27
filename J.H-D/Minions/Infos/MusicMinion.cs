using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

using J.H_D.Data;

using MusicArtist = J.H_D.Data.Response.MusicArtist;
using System.Globalization;
using System;
using static J.H_D.Data.Response;
using J.H_D.Data.Exceptions;
using J.H_D.Tools;

namespace J.H_D.Minions.Infos
{
    static class MusicMinion
    {
        private const string RootUrl = "http://ws.audioscrobbler.com/2.0/";
        private const string LyricsRootApi = "https://api.musixmatch.com/ws/1.1/";

        private const string LyricsSection = "class=\"hwc\"><div class=\"BNeawe tAd8D AP7Wnd\"><div><div class=\"BNeawe tAd8D AP7Wnd\">";
        private const string ArtistSection = "class=\"BNeawe s3v9rd AP7Wnd\">";
        private const string TitleSection = "class=\"BNeawe tAd8D AP7Wnd\"";

        public static async Task<FeatureRequest<MusicArtist?, Error.Brainz>> LookForArtistAsync(string GroupName)
        {
            MusicArtist? Artist = null;
            
            dynamic Json;
            Json = JsonConvert.DeserializeObject(await JHConfig.Asker.GetStringAsync($"{RootUrl}?method=artist.getinfo&artist={GroupName}&api_key={JHConfig.APIKey["LastFM"]}&format=json"));

            dynamic ArtistInfos = Json.artist;
            
            Artist = new MusicArtist
            {
                Name = ArtistInfos.name,
                Mbid = ArtistInfos.mbid,
                LastUrl = new Uri((string)ArtistInfos.url),
                OnTour = (string)ArtistInfos.ontour != "0",
                Genres = GetTags(ArtistInfos.tags),
                Bio = ArtistInfos.bio.summary
            };

            return new FeatureRequest<MusicArtist?, Error.Brainz>(Artist, Error.Brainz.None);
        }
        
        public static async Task<FeatureRequest<SongLyrics?, Error.LyricsMatch>> GetLyrics(string[] Args)
        {
            if (Args.Length == 0)
                return new FeatureRequest<SongLyrics?, Error.LyricsMatch>(null, Error.LyricsMatch.Help);

            string UrlArtist = String.Join("+", Args);
            string Results = await JHConfig.Asker.GetStringAsync($"https://google.com/search?q={UrlArtist}+lyrics");

            try
            {
                return new FeatureRequest<SongLyrics?, Error.LyricsMatch>(new SongLyrics
                {
                    Lyrics = Utilities.GetHTMLSection(Results, LyricsSection, "</div>"),
                    Artist = Utilities.GetHTMLSection(Results, ArtistSection, "</span>", 2),
                    SongName = Utilities.GetHTMLSection(Results, TitleSection, "</span>")
                }, Error.LyricsMatch.None);
            }
            catch (Exception e) when (e is SectionNotFoundException)
            {
                return new FeatureRequest<SongLyrics?, Error.LyricsMatch>(null, Error.LyricsMatch.NotFound);
            }
        }

        private static List<string> GetTags(dynamic TagsSection)
        {
            List<string> Tags = new List<string>();

            foreach (dynamic tag in TagsSection.tag)
            {
                string ChangedTag = (string)tag.name;
                ChangedTag = char.ToUpper(ChangedTag[0], CultureInfo.InvariantCulture) + ChangedTag.Substring(1);
                Tags.Add(ChangedTag);
            }

            return Tags;
        }
    }
}
