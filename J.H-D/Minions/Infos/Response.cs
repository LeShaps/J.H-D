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
            [Embedable("Name")]
            public string SeriesName;
            [Embedable("Id", false)]
            public string SeriesId;
            [Embedable("BackPoster", false, true)]
            public string BackdropPath;
            [Embedable("EpisodeLenght")]
            public string EpisodeRunTime;
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
            [Embedable("Synopsis")]
            public string Overview;
            [Embedable("Seasons", false, false)]
            public List<TVSeason> Seasons;

            [Embedable("RessourceLink", false, false)]
            public readonly string RessourcePath = "https://image.tmdb.org/t/p/w300";
        }

        public class TVSeason
        {
            [Embedable("OriginalSeriesName")]
            public string FirstSeriesName;
            [Embedable("OriginalSeriesId", false)]
            public string FirstSeriesId;
            [Embedable("Number of Episode")]
            public string EpisodeNumber;
            [Embedable("SeasonID", false)]
            public string Id;
            [Embedable("Synopsis")]
            public string Overview;
            [Embedable("Poster", false, true)]
            public string PosterPath;
            [Embedable("Season Number")]
            public string SNumber;
            [Embedable("Season Name")]
            public string SName;

            [Embedable("RessourceLink", false, false)]
            public readonly string RessourcePath = "https://image.tmdb.org/t/p/w300";
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

        public class Anime
        {
            [Embedable("Anime ID", false, false)]
            public string Id;
            [Embedable("Synopsis")]
            public string Synopsis;
            [Embedable("English Title")]
            public string Title;
            [Embedable("Latin Title")]
            public string LATitle;
            [Embedable("Japanese Title")]
            public string OriginalTitle;
            [Embedable("Average Rating")]
            public string Rating;
            [Embedable("Start Date")]
            public string StartDate;
            [Embedable("End Date")]
            public string EndDate;
            [Embedable("Age Rating")]
            public string AgeRating;
            [Embedable("Age Reasons")]
            public string Guideline;
            [Embedable("Status")]
            public string Status;
            [Embedable("Poster", false, true)]
            public string PosterImage;
            [Embedable("Cover", false, true)]
            public string CoverImage;
            [Embedable("Episode Count")]
            public string EpisodeCount;
            [Embedable("Episode Lenght")]
            public string EpLength;
            [Embedable("Total Watchtime")]
            public string HumanReadableWatchtime;
            [Embedable("Video Url", false, true)]
            public string VideoUrl;
        }
    }
}
