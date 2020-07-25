using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Minions.Responses
{
    public static partial class Response
    {
        public class Movie
        {
            public string Name;
            public string PosterPath;
            public bool Adult;
            public string Overview;
            public string ReleaseDate;
            // public DateTime ReleaseDate;
            public List<string> Genres;
            public uint Id;
            public string OriginalTitle;
            public string OriginalLanguage;
            public string BackdropPath;
            public string AverageNote;
            public uint Budget;
            public uint Revenue;
            public List<string> ProductionCompanies;
            public string Runtime;

            public readonly string RessourcePath = "https://image.tmdb.org/t/p/w300";
        }

        public class TVSeries
        {
            public string BackdropPath;
            public int EpisodeRunTime;
            public string Started;
            public List<string> Genres;
            public bool InProduction;
            public string HomePage;
            public string SeasonNumber;
            public string EpisodeNumber;
            public List<string> Compagnies;
            public string VoteAverage;
        }

        public class UrbanDefinition
        {
            public string Word;
            public string Definition;
            public string Link;
            public string Author;
            public string Exemples;
        }

        public enum AreaType
        {
            City,
            Building,
            Street
        }

        public class MusicArtist
        {
            public string Name;
            public string MBID;
            public string LastUrl;
            public string ImageUrl;
            public List<string> Genres;
            public string Bio;
            public bool OnTour;
        }
    }
}
