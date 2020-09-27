using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace J.H_D.Data
{
    public static partial class Response
    {
        public class Movie
        {
            private const string V = "https://image.tmdb.org/t/p/w300";

            [Embedable("Name")]
            private string name;
            [Embedable("Poster", false, true)]
            private string posterPath;
            [Embedable("Adult", false)]
            private bool adult;
            [Embedable("Synopsis")]
            private string overview;
            [Embedable("Date")]
            private string releaseDate;
            [Embedable("Genres")]
            private ICollection<string> genres;
            [Embedable("MovieID", false)]
            private uint id;
            [Embedable("OriginalName")]
            private string originalTitle;
            [Embedable("OriginalLanguage")]
            private string originalLanguage;
            [Embedable("BackPoster", false, true)]
            private string backdropPath;
            [Embedable("Note")]
            private string averageNote;
            [Embedable("Budget")]
            private uint budget;
            [Embedable("Revenue")]
            private uint revenue;
            [Embedable("Compagnies")]
            private ICollection<string> productionCompanies;
            [Embedable("Runtime")]
            private string runtime;

            [Embedable("RessourceLink", false, false)]
            private readonly string ressourcePath = V;

            public string Name { get => name; set => name = value; }
            public string PosterPath { get => posterPath; set => posterPath = value; }
            public bool Adult { get => adult; set => adult = value; }
            public string Overview { get => overview; set => overview = value; }
            public string ReleaseDate { get => releaseDate; set => releaseDate = value; }
            public ICollection<string> Genres { get => genres; set => genres = value; }
            public uint Id { get => id; set => id = value; }
            public string OriginalTitle { get => originalTitle; set => originalTitle = value; }
            public string OriginalLanguage { get => originalLanguage; set => originalLanguage = value; }
            public string BackdropPath { get => backdropPath; set => backdropPath = value; }
            public string AverageNote { get => averageNote; set => averageNote = value; }
            public uint Budget { get => budget; set => budget = value; }
            public uint Revenue { get => revenue; set => revenue = value; }
            public ICollection<string> ProductionCompanies { get => productionCompanies; set => productionCompanies = value; }
            public string Runtime { get => runtime; set => runtime = value; }

            public string RessourcePath => ressourcePath;
        }

        public class TVSeries
        {
            private const string V = "https://image.tmdb.org/t/p/w300";

            [Embedable("Name")]
            private string seriesName;
            [Embedable("Id", false)]
            private string seriesId;
            [Embedable("BackPoster", false, true)]
            private string backdropPath;
            [Embedable("EpisodeLenght")]
            private string episodeRunTime;
            [Embedable("Start Date")]
            private string started;
            [Embedable("Genres")]
            private ICollection<string> genres;
            [Embedable("In Production")]
            private bool inProduction;
            [Embedable("Website", true, true)]
            private string homePage;
            [Embedable("Season Count")]
            private string seasonNumber;
            [Embedable("Episode Count")]
            private string episodeNumber;
            [Embedable("Compagnies")]
            private ICollection<string> compagnies;
            [Embedable("Note")]
            private string voteAverage;
            [Embedable("Synopsis")]
            private string overview;
            [Embedable("Seasons", false, false)]
            private ICollection<TVSeason> seasons;

            [Embedable("RessourceLink", false, false)]
            private readonly string ressourcePath = V;

            public string SeriesName { get => seriesName; set => seriesName = value; }
            public string SeriesId { get => seriesId; set => seriesId = value; }
            public string BackdropPath { get => backdropPath; set => backdropPath = value; }
            public string EpisodeRunTime { get => episodeRunTime; set => episodeRunTime = value; }
            public string Started { get => started; set => started = value; }
            public ICollection<string> Genres { get => genres; set => genres = value; }
            public bool InProduction { get => inProduction; set => inProduction = value; }
            public string HomePage { get => homePage; set => homePage = value; }
            public string SeasonNumber { get => seasonNumber; set => seasonNumber = value; }
            public string EpisodeNumber { get => episodeNumber; set => episodeNumber = value; }
            public ICollection<string> Compagnies { get => compagnies; set => compagnies = value; }
            public string VoteAverage { get => voteAverage; set => voteAverage = value; }
            public string Overview { get => overview; set => overview = value; }
            public ICollection<TVSeason> Seasons { get => seasons; set => seasons = value; }

            public string RessourcePath => ressourcePath;
        }

        public class TVSeason
        {
            private const string V = "https://image.tmdb.org/t/p/w300";

            [Embedable("OriginalSeriesName")]
            private string firstSeriesName;
            [Embedable("OriginalSeriesId", false)]
            private string firstSeriesId;
            [Embedable("Number of Episode")]
            private string episodeNumber;
            [Embedable("SeasonID", false)]
            private string id;
            [Embedable("Synopsis")]
            private string overview;
            [Embedable("Poster", false, true)]
            private string posterPath;
            [Embedable("Season Number")]
            private string sNumber;
            [Embedable("Season Name")]
            private string sName;

            [Embedable("RessourceLink", false, false)]
            private readonly string ressourcePath = V;

            public string FirstSeriesName { get => firstSeriesName; set => firstSeriesName = value; }
            public string FirstSeriesId { get => firstSeriesId; set => firstSeriesId = value; }
            public string EpisodeNumber { get => episodeNumber; set => episodeNumber = value; }
            public string Id { get => id; set => id = value; }
            public string Overview { get => overview; set => overview = value; }
            public string PosterPath { get => posterPath; set => posterPath = value; }
            public string SNumber { get => sNumber; set => sNumber = value; }
            public string SName { get => sName; set => sName = value; }

            public string RessourcePath => ressourcePath;
        }

        public struct SongLyrics : IEquatable<SongLyrics>
        {
            private string lyrics;
            private string artist;
            private string songName;

            public string Lyrics { get => lyrics; set => lyrics = value; }
            public string Artist { get => artist; set => artist = value; }
            public string SongName { get => songName; set => songName = value; }

            public override bool Equals(object obj)
            {
                return obj is SongLyrics lyrics &&
                       Lyrics == lyrics.Lyrics;
            }

            public bool Equals([AllowNull] SongLyrics other)
            {
                return Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Lyrics);
            }

            public static bool operator ==(SongLyrics left, SongLyrics right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(SongLyrics left, SongLyrics right)
            {
                return !(left == right);
            }
        }

        public struct UrbanDefinition : IEquatable<UrbanDefinition>
        {
            [Embedable("Word")]
            private string word;
            [Embedable("Definition")]
            private string definition;
            [Embedable("Link", false, true)]
            private string link;
            [Embedable("Author")]
            private string author;
            [Embedable("Exemples")]
            private string exemples;

            public string Word { get => word; set => word = value; }
            public string Definition { get => definition; set => definition = value; }
            public string Link { get => link; set => link = value; }
            public string Author { get => author; set => author = value; }
            public string Exemples { get => exemples; set => exemples = value; }

            public bool Equals(UrbanDefinition other)
            {
                return
                    GetHashCode() == other.GetHashCode();
            }
        }

        public enum AreaType
        {
            City,
            Building,
            Street
        }

        public struct MusicArtist : IEquatable<MusicArtist>
        {
            [Embedable("Artist Name")]
            private string name;
            [Embedable("ID", false, false)]
            private string mBID;
            [Embedable("Band Page", false, true)]
            private Uri lastUrl;
            [Embedable("Image Url", false, true)]
            private Uri imageUrl;
            [Embedable("Genres")]
            private ICollection<string> genres;
            [Embedable("Bio")]
            private string bio;
            [Embedable("Active")]
            private bool onTour;

            public string Name { get => name; set => name = value; }
            public string Mbid { get => mBID; set => mBID = value; }
            public Uri LastUrl { get => lastUrl; set => lastUrl = value; }
            public Uri ImageUrl { get => imageUrl; set => imageUrl = value; }
            public ICollection<string> Genres { get => genres; set => genres = value; }
            public string Bio { get => bio; set => bio = value; }
            public bool OnTour { get => onTour; set => onTour = value; }

            public bool Equals(MusicArtist other)
            {
                return
                    GetHashCode() == other.GetHashCode();
            }
        }
        
        public struct Anime : IEquatable<Anime>
        {
            [Embedable("Anime ID", false, false)]
            private string id;
            [Embedable("Synopsis")]
            private string synopsis;
            [Embedable("English Title")]
            private string title;
            [Embedable("Latin Title")]
            private string lATitle;
            [Embedable("Japanese Title")]
            private string originalTitle;
            [Embedable("Average Rating")]
            private string rating;
            [Embedable("Start Date")]
            private string startDate;
            [Embedable("End Date")]
            private string endDate;
            [Embedable("Age Rating")]
            private string ageRating;
            [Embedable("Age Reasons")]
            private string guideline;
            [Embedable("Status")]
            private string status;
            [Embedable("Poster", false, true)]
            private string posterImage;
            [Embedable("Cover", false, true)]
            private string coverImage;
            [Embedable("Episode Count")]
            private string episodeCount;
            [Embedable("Episode Lenght")]
            private string epLength;
            [Embedable("Total Watchtime")]
            private string humanReadableWatchtime;
            [Embedable("Video Url", false, true)]
            private Uri videoUrl;

            public string Id { get => id; set => id = value; }
            public string Synopsis { get => synopsis; set => synopsis = value; }
            public string Title { get => title; set => title = value; }
            public string LATitle { get => lATitle; set => lATitle = value; }
            public string OriginalTitle { get => originalTitle; set => originalTitle = value; }
            public string Rating { get => rating; set => rating = value; }
            public string StartDate { get => startDate; set => startDate = value; }
            public string EndDate { get => endDate; set => endDate = value; }
            public string AgeRating { get => ageRating; set => ageRating = value; }
            public string Guideline { get => guideline; set => guideline = value; }
            public string Status { get => status; set => status = value; }
            public string PosterImage { get => posterImage; set => posterImage = value; }
            public string CoverImage { get => coverImage; set => coverImage = value; }
            public string EpisodeCount { get => episodeCount; set => episodeCount = value; }
            public string EpLength { get => epLength; set => epLength = value; }
            public string HumanReadableWatchtime { get => humanReadableWatchtime; set => humanReadableWatchtime = value; }
            public Uri VideoUrl { get => videoUrl; set => videoUrl = value; }

            public bool Equals(Anime other)
            {
                return
                    GetHashCode() == other.GetHashCode();
            }
        }
    }
}
