using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Minions.Infos
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
            public string[] Genres;
            public uint Id;
            public string OriginalTitle;
            public string OriginalLanguage;
            public string BackdropPath;
            public string AverageNote;
            public uint Budget;
            public uint Revenue;

            public readonly string RessourcePath = "https://image.tmdb.org/t/p/w300";
        }
    }
}
