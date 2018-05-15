using Discord.Commands;
using Discord;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace J.H_D
{
    class Anime
    {
        public string _name { private set; get; }
        public uint _nb { private set; get; }
        public string _rate { private set; get; }
        public string _startDate { private set; get; }
        public string _endDate { private set; get; }
        public string _synopsis { private set; get; }
        public string _image_link { private set; get; }
        
        public Anime() { }

        public Anime(string name, uint nb, string rate, string startDate, string endDate, string synopsis, string image_link)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _nb = nb;
            _rate = rate ?? throw new ArgumentNullException(nameof(rate));
            _startDate = startDate ?? throw new ArgumentNullException(nameof(startDate));
            _endDate = endDate ?? throw new ArgumentNullException(nameof(endDate));
            _synopsis = synopsis ?? throw new ArgumentNullException(nameof(synopsis));
            _image_link = image_link ?? throw new ArgumentNullException(nameof(image_link));
        }

        public Anime(string xml)
        {
            _name = Program.getInfos("<title>", xml, '<');
            _nb = Convert.ToUInt16(Program.getInfos("<episodes>", xml, '<'));
            _rate = Program.getInfos("<score>", xml, '<');
            _startDate = Program.getInfos("<start_date>", xml, '<');
            _endDate = Program.getInfos("<end_date>", xml, '<');
            _synopsis = Program.getInfos("<synopsis>", xml, '<');
            _image_link = Program.getInfos("<image>", xml, '<');

            if (_synopsis != null)
            {
                _synopsis = Program.removeSymbols(_synopsis);
                if (_synopsis.Length > 1024)
                {
                    _synopsis = _synopsis.Substring(0, 1021) + "...";
                }
            }
        }
    }

    class Manga
    {
        public string _name { private set; get; }
        public string _volumes { private set; get; }
        public string _chapters { private set; get; }
        public string _rate { private set; get; }
        public string _startDate { private set; get; }
        public string _endDate { private set; get; }
        public string _sysnopsis { private set; get; }

        public Manga() { }
    }

    class MalModule : ModuleBase
    {
        Program p = Program.p;
        Dictionary<ulong, Anime> last_animes = new Dictionary<ulong, Anime>();
        Dictionary<ulong, Manga> last_mangas = new Dictionary<ulong, Manga>();

        [Command("Get anime")]
        public async Task get_anime(params string[] Args)
        {
            ulong UserId = Context.User.Id;
            string searchterms = Program.makeArgs(Args);
            string xmlRequest;

            searchterms.Replace(" ", "+");
            xmlRequest = Program.malAsker.DownloadString("https://myanimelist.net/api/anime/search.xml?q=" + searchterms);

            File.WriteAllText("MALres.xml", xmlRequest);
            if (last_animes != null)
            {
                if (last_animes.ContainsKey(UserId))
                {
                    last_animes[UserId] = new Anime(xmlRequest);
                    if (Havenullstring(last_animes[UserId]) == true)
                    {
                        await ReplyAsync("J'ai bien peur de ne pas connaître cet anime, au cas où, veuillez vérifier l'ortographe");
                        return;
                    }
                }
                else
                {
                    last_animes.Add(Context.User.Id, new Anime(xmlRequest));
                    if (Havenullstring(last_animes[UserId]) == true)
                    {
                        await ReplyAsync("J'ai bien peur de ne pas connaître cet anime, au cas où, veuillez vérifier l'ortographe");
                        return;
                    }
                }
            }
            await ReplyAsync("Voici des infos sur l'anime que vous recherchez :", false, Speetch.anime_builder(last_animes[UserId]).Build());
        }

        private bool Havenullstring(Anime anime)
        {
            if (anime._name == null || anime._rate == null || anime._startDate == null || anime._synopsis == null ||
                anime._endDate == null || anime._image_link == null)
                return (true);
            return (false);
        }
    }
}
