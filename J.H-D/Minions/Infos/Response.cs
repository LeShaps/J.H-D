using J.H_D.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Data
{
    public static partial class Response
    {
        public class Movie
        {
            [Embedable("Name")]
            public string Name;
            [Embedable("Poster", false, true)]
            public string PosterPath;
            [Embedable("Adult", false)]
            public bool Adult;
            [Embedable("Synopsis")]
            public string Overview;
            [Embedable("Date")]
            public string ReleaseDate;
            [Embedable("Genres")]
            public List<string> Genres;
            [Embedable("MovieID", false)]
            public uint Id;
            [Embedable("OriginalName")]
            public string OriginalTitle;
            [Embedable("OriginalLanguage")]
            public string OriginalLanguage;
            [Embedable("BackPoster", false, true)]
            public string BackdropPath;
            [Embedable("Note")]
            public string AverageNote;
            [Embedable("Budget")]
            public uint Budget;
            [Embedable("Revenue")]
            public uint Revenue;
            [Embedable("Compagnies")]
            public List<string> ProductionCompanies;
            [Embedable("Runtime")]
            public string Runtime;

            [Embedable("RessourceLink", false, false)]
            public readonly string RessourcePath = "https://image.tmdb.org/t/p/w300";
        }

        public class TVSeries
        {
            [Embedable("BackPoster", false, true)]
            public string BackdropPath;
            [Embedable("EpisodeLenght")]
            public int EpisodeRunTime;
            [Embedable("Start Date")]
            public string Started;
            [Embedable("Genres")]
            public List<string> Genres;
            [Embedable("In Production")]
            public bool InProduction;
            [Embedable("Website", Link:true)]
            public string HomePage;
            [Embedable("Season Count")]
            public string SeasonNumber;
            [Embedable("Episode Count")]
            public string EpisodeNumber;
            [Embedable("Compagnies")]
            public List<string> Compagnies;
            [Embedable("Note")]
            public string VoteAverage;
        }

        public class UrbanDefinition
        {
            [Embedable("Word")]
            public string Word;
            [Embedable("Definition")]
            public string Definition;
            [Embedable("Link", false, true)]
            public string Link;
            [Embedable("Author")]
            public string Author;
            [Embedable("Exemples")]
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
            [Embedable("Artist Name")]
            public string Name;
            [Embedable("ID", false, false)]
            public string MBID;
            [Embedable("Band Page", false, true)]
            public string LastUrl;
            [Embedable("Image Url", false, true)]
            public string ImageUrl;
            [Embedable("Genres")]
            public List<string> Genres;
            [Embedable("Bio")]
            public string Bio;
            [Embedable("Active")]
            public bool OnTour;
        }
    }
}
