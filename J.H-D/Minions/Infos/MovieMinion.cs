using Discord;
using J.H_D.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Minions.Responses
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

        private static readonly Dictionary<SearchType, string> EndpointList = new Dictionary<SearchType, string>()
        {
            {SearchType.Movie, "https://api.themoviedb.org/3/search/movie?api_key=" },
            {SearchType.Serie, "https://api.themoviedb.org/3/search/tv?api_key=" },
            {SearchType.Director, "https://api.themoviedb.org/3/search/person?api_key=" }
        };

        public static async Task<FeatureRequest<Response.Movie, Error.Movie>> SearchMovie(string[] args)
        {
            string RequestName = Utilities.MakeQueryArgs(args);
            if (RequestName.Length == 0)
                return (new FeatureRequest<Response.Movie, Error.Movie>(null, Error.Movie.Help));

            dynamic Json;
            using (HttpClient hc = new HttpClient())
            {
                Json = JsonConvert.DeserializeObject(await (await hc.GetAsync($"https://api.themoviedb.org/3/search/movie?api_key={Program.p.TmDbKey}&language=en-US&query={RequestName}&page=1&include_adult=false")).Content.ReadAsStringAsync());
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

        public static async Task<FeatureRequest<Response.Movie, Error.Movie>> BonusInfos(SearchType searchType, string[] args)
        {
            string RequestName = Utilities.MakeQueryArgs(args);
            if (RequestName.Length == 0)
                return new FeatureRequest<Response.Movie, Error.Movie>(null, Error.Movie.Help);
            
            dynamic Moviejson;
            using (HttpClient client = new HttpClient())
            {
                Moviejson = JsonConvert.DeserializeObject(await (await client.GetAsync($"https://api.themoviedb.org/3/search/movie?api_key={Program.p.TmDbKey}&language=en-US&query={RequestName}")).Content.ReadAsStringAsync());
            }

            if (Moviejson["total_results"] == "0")
                return new FeatureRequest<Response.Movie, Error.Movie>(null, Error.Movie.NotFound);

            JArray Results = (JArray)Moviejson["results"];
            dynamic MovieResults = Results[0];
            dynamic DetailsJson;
            
            using (HttpClient client = new HttpClient())
            {
                DetailsJson = JsonConvert.DeserializeObject(await (await client.GetAsync($"https://api.themoviedb.org/3/movie/{MovieResults.id}?api_key={Program.p.TmDbKey}&language=en-US")).Content.ReadAsStringAsync());
            }

            return new FeatureRequest<Response.Movie, Error.Movie>(new Response.Movie()
            {
                Name = DetailsJson.original_title,
                PosterPath = DetailsJson.poster_path,
                Adult = DetailsJson.adult,
                Overview = DetailsJson.overview,
                ReleaseDate = DetailsJson.release_date,
                Id = DetailsJson.id,
                OriginalTitle = DetailsJson.original_title,
                OriginalLanguage = DetailsJson.original_language,
                BackdropPath = DetailsJson.backdrop_path,
                AverageNote = DetailsJson.vote_average,
                Budget = DetailsJson.budget,
                Revenue = DetailsJson.revenue,
                ProductionCompanies = GetNames(DetailsJson.production_companies),
                Genres = GetNames(DetailsJson.genres),
                Runtime = GetRuntime((int)DetailsJson.runtime)
            }, Error.Movie.None);
        }

        private static string GetRuntime(int minutes)
        {
            TimeSpan span = TimeSpan.FromMinutes(minutes);
            return span.ToString(@"hh\:mm");
        }

        private static List<string> GetNames(dynamic DynamicArray)
        {
            List<string> results = new List<string>();
            if (DynamicArray == null) return null;

            foreach (dynamic d in DynamicArray)
            {
                results.Add((string)d.name);
            }

            return results;
        }
    }
}
