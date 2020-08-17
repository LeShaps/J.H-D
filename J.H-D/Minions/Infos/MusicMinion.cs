﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

using J.H_D.Data;

using MusicArtist = J.H_D.Data.Response.MusicArtist;
using System.Globalization;
using System;

namespace J.H_D.Minions.Infos
{
    static class MusicMinion
    {
        private const string RootUrl = "http://ws.audioscrobbler.com/2.0/";

        public static async Task<FeatureRequest<MusicArtist?, Error.Brainz>> LookForArtistAsync(string GroupName)
        {
            MusicArtist? Artist = null;
            
            dynamic Json;
            Json = JsonConvert.DeserializeObject(await Program.GetP().Asker.GetStringAsync($"{RootUrl}?method=artist.getinfo&artist={GroupName}&api_key={Program.GetP().LastFMKey}&format=json"));

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
