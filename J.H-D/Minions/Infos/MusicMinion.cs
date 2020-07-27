using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver.Model;

using J.H_D.Data;

namespace J.H_D.Minions.Infos
{
    class MusicMinion
    {
        private static readonly string RootUrl = "http://ws.audioscrobbler.com/2.0/";

        public static async Task<FeatureRequest<Response.MusicArtist, Error.Brainz>> LookForArtist(string GroupName)
        {
            Response.MusicArtist Artist = null;
            
            dynamic Json;
            Json = JsonConvert.DeserializeObject(await Program.p.Asker.GetStringAsync($"{RootUrl}?method=artist.getinfo&artist={GroupName}&api_key={Program.p.LastFMKey}&format=json"));

            dynamic ArtistInfos = Json.artist;

            Artist = new Response.MusicArtist()
            {
                Name = ArtistInfos.name,
                MBID = ArtistInfos.mbid,
                LastUrl = ArtistInfos.url,
                OnTour = (string)ArtistInfos.ontour == "0" ? false : true,
                Genres = GetTags(ArtistInfos.tags),
                Bio = ArtistInfos.bio.summary
            };

            return new FeatureRequest<Response.MusicArtist, Error.Brainz>(Artist, Error.Brainz.None);
        }

        private static List<string> GetTags(dynamic TagsSection)
        {
            List<string> Tags = new List<string>();

            foreach (dynamic tag in TagsSection.tag)
            {
                string ChangedTag = (string)tag.name;
                ChangedTag = char.ToUpper(ChangedTag[0]) + ChangedTag.Substring(1);
                Tags.Add(ChangedTag);
            }

            return Tags;
        }
    }
}
