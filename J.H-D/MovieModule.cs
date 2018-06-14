using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D
{
    partial class Movie
    {
        public string _name { private set; get; }
        public string _posterpath { private set; get; }
        public bool _adult { private set; get; }
        public string _overview { private set; get; }
        public string _releaseDate { private set; get; }
        public string[] _genres { private set; get; }
        public uint _id { private set; get; }
        public string _originalTitle { private set; get; }
        public string _originalLanguage { private set; get; }
        public string _backdropPath { private set; get; }
        public string _averageNote { private set; get; }
        public uint _budget { private set; get; }
        public uint _revenue { private set; get; }

        public Movie() { }

        public Movie(string source, string bonus)
        {
            _name = Program.getInfos("\"title\":\"", source, '"');
            _posterpath = "https://image.tmdb.org/t/p/w300" + Program.getInfos("\"poster_path\":\"\\", source, '"');
            if (Program.getInfos("\"adult\":\"", source, ',') == "true")
                _adult = true;
            else
                _adult = false;
            _overview = Program.getInfos("\"overview\":\"", source, '"');
            _releaseDate = Program.getInfos("\"release_date\":\"", source, '"');
            _genres = Program.getInfos("\"genre_ids\":[", source, ']').Split(',');
            _id = Convert.ToUInt32(Program.getInfos("\"id\":\"", source, ','));
            _originalTitle = Program.getInfos("\"original_title\":\"", source, '"');
            _originalLanguage = Program.getInfos("\"original_language\":\"", source, '"');
            _backdropPath = "https://image.tmdb.org/t/p/w300" + Program.getInfos("\"backdrop_path\":\"\\", source, '"');
            _averageNote = Program.getInfos("\"vote_average\":", source, ',');
            if (_overview == null)
                _overview = Program.getInfos("\"overview\":\"", bonus, '"');
            _budget = Convert.ToUInt32(Program.getInfos("\"budget\":", bonus, ','));
            _revenue = Convert.ToUInt32(Program.getInfos("\"revenue\":", bonus, ','));
        }

        public Movie fill_movie(string source, string bonus)
        {
            _name = Program.getInfos("\"title\":\"", source, '"');
            _posterpath = "https://image.tmdb.org/t/p/w300" + Program.getInfos("\"poster_path\":\"", source, ',');
            if (Program.getInfos("\"adult\":\"", source, ',') == "true")
                _adult = true;
            else
                _adult = false;
            _overview = Program.getInfos("\"overview\":\"", source, '"');
            _releaseDate = Program.getInfos("\"release_date\":\"", source, '"');
            _genres = Program.getInfos("\"genre_ids\":\"[", source, ']').Split(',');
            _id = Convert.ToUInt32(Program.getInfos("\"id\":\"", source, ','));
            _originalTitle = Program.getInfos("\"original_title\":\"", source, '"');
            _originalLanguage = Program.getInfos("\"original_language\":\"", source, '"');
            _backdropPath = "https://image.tmdb.org/t/p/w300" + Program.getInfos("\"backdrop_path\":\"", source, ',');
            _averageNote = Program.getInfos("\"vote_average\":", source, ',');
            _budget = Convert.ToUInt32(Program.getInfos("\"budget\":", bonus, ','));
            _revenue = Convert.ToUInt32(Program.getInfos("\"revenue\":", bonus, ','));

            return (this);
        }
    }

    partial class MovieModule : ModuleBase
    {
        Program p = Program.p;
        static Dictionary<ulong, Movie> lastfilms = new Dictionary<ulong, Movie>();

        [Command("Get last movie")]
        public async Task MoreInfos(params string[] Args)
        {
            if (Args[0] == "infos")
            {
                if (lastfilms != null)
                {
                    if (lastfilms.ContainsKey(Context.User.Id))
                    {
                        await ReplyAsync("Voici plus d'info sur le film", false, Speetch.makemovie_moreinfo(lastfilms[Context.User.Id]).Build());
                        return;
                    }
                    else
                    {
                        await ReplyAsync("Vous n'avez pas encore demandé de film cette section");
                        return;
                    }
                }
                else
                {
                    await ReplyAsync("Je n'ai pas encore envoyé de film cette section");
                    return;
                }
            }
        }

        [Command("Get movie")]
        public async Task GetMovie(params string[] Args)
        {
            string tosearch = Program.makeArgs(Args).Replace(" ", "%20");
            string key = File.ReadAllLines("Logers/tmdbkey.txt")[0];

            string basics = p.callers.SimpleAsk("https://api.themoviedb.org/3/search/movie?api_key=" + key + "&language=fr-FR&query=" + tosearch + "&page=1&include_adult=true");
            string id = Program.getInfos("\"id\":", basics, ',');
            if (id == null)
            {
                await ReplyAsync(Speetch.NfMovie);
                return;
            }
            string bonus = p.callers.SimpleAsk("https://api.themoviedb.org/3/movie/" + id + "?api_key=" + key + "&language=en-US");
            Console.WriteLine(bonus);

            Movie currMovie = new Movie(basics, bonus);
            if (lastfilms.ContainsKey(Context.User.Id))
            {
                lastfilms[Context.User.Id] = currMovie;
            }
            else
            {
                lastfilms.Add(Context.User.Id, currMovie);
            }
            await ReplyAsync("Voici des infos sur le film que vous voulez", false, Speetch.make_movieinfos(currMovie).Build());
        }
    }
}
