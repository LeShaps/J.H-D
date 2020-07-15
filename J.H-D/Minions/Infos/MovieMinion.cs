using Discord;
using J.H_D.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Minions.Infos
{
    public static class MovieMinion
    {
        public enum SearchType
        {
            Movie,
            Serie,
            Director
        }

        private static bool ContainNullCheck(string s1, string s2)
        {
            if (s1 == null)
                return false;
            return s1.Contains(s2);
        }

        public static async Task<FeatureRequest<Response.Movie, Error.Movie>> SearchMovie(SearchType searchType, string[] args, Dictionary<string, string> Auth)
        {
            string RequestName = Utilities.MakeQueryArgs(args);
            if (RequestName.Length == 0)
                return (new FeatureRequest<Response.Movie, Error.Movie>(null, Error.Movie.Help));

            dynamic Json;
            using (HttpClient hc = new HttpClient())
            {
                Json = JsonConvert.DeserializeObject(await (await hc.GetAsync("https://api.themoviedb.org/3/search/movie?api_key=" + Program.p.TmDbKey + "&language=en-US&query=" + RequestName + "&page=1&include_adult=false")).Content.ReadAsStringAsync());
            }
            if (Json["total_results"] == "0")
                return new FeatureRequest<Response.Movie, Error.Movie>(null, Error.Movie.NotFound);
            JArray Results = (JArray)Json["results"];
            dynamic FinalData = Results[0];
            return new FeatureRequest<Response.Movie, Error.Movie>(new Response.Movie()
            {
                Name = FinalData.original_title,
                PosterPath = FinalData.poster_path,
                Adult = FinalData.adult,
                Overview = FinalData.overview,
                ReleaseDate = FinalData.release_date,
                Id = FinalData.id,
                OriginalTitle = FinalData.original_title,
                OriginalLanguage = FinalData.original_language,
                BackdropPath = FinalData.backdrop_path,
                AverageNote = FinalData.vote_average,
            }, Error.Movie.None);
        }
    }
}
