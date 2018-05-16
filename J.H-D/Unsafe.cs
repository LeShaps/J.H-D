using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace J.H_D
{
    enum TagType
    {
        General,
        Artist,
        Copyright,
        Character
    }

    public class Image_s
    {
        public string _file_url { protected set; get; }
        public string _sample_url { protected set; get; }
        public string _preview_url { protected set; get; }
        public char _rating { protected set; get; }
        public bool _isLoli { protected set; get; }
        public string _source { protected set; get; }
        public string _name { protected set; get; }
    }

    class Tag
    {
        public TagType _type { private set; get; }
        public string _name { private set; get; }
        public uint _id { private set; get; }
    }

    class sk_image : Image_s
    {
        public string _author { private set; get; }
        public string _file_furl { private set; get; }
        public List<Tag> _tags { private set; get; }

        public sk_image() { }

        public sk_image(string source)
        {
            _file_url = Program.getInfos("\"file_url\":\"//", source, '"');
            _sample_url = Program.getInfos("\"sample_url\":\"//", source, '"');
            _preview_url = Program.getInfos("\"preview_url\":\"//", source, '"');
            _rating = Program.getInfos("\"rating\":\"", source, '"')[0];
            if (Program.getInfos("\"has_children\":", source, ',') == "false")
                _isLoli = false;
            else
                _isLoli = true;
            _source = Program.getInfos("\"source\":\"", source, '"');
            _author = Program.getInfos("\"author\":\"", source, '"');
            make_taglist(Program.getInfos("\"tags\":[", source, ']'));
            make_name(_file_url);
        }

        private void make_name(string url)
        {
            string[] fparsed = url.Split('.');
            string extension = fparsed[fparsed.Length - 1];
            string[] parsed_fname = fparsed[fparsed.Length - 2].Split('/');
            string fileurl = parsed_fname[parsed_fname.Length - 1] + "." + extension;
            string name = fileurl.Split('?')[0];

            _file_furl = "https://" + url;
            _name = name;
        }

        public string make_tagnamelist(List<Tag> tags)
        {
            return ("");
        }
        
        private void make_taglist(string ttp)
        {
            List<Tag> tags = new List<Tag>();

            _tags = tags;
        }
    }

    class r34_image : Image_s
    {
        public string[] _tags { private set; get; }

        public r34_image() { }

        public r34_image(string source)
        {
            _file_url = Program.getInfos("file_url=\"", source, '"');
            _sample_url = Program.getInfos("sample_url=\"", source, '"');
            _preview_url = Program.getInfos("preview_url=\"", source, '"');
            _rating = Program.getInfos("rating=\"", source, '"')[0];
            if (Program.getInfos("has_children=\"", source, '"') == "false")
                _isLoli = false;
            else
                _isLoli = true;
            _source = Program.getInfos("source=\"", source, '"');
            _tags = Program.getInfos("tags=\"", source, '"').Split(' ');
        }
    }

    class UnsafeModule : ModuleBase
    {
        Program p = Program.p;
        static r34_image lastr_image;
        static Dictionary<ulong, sk_image> lastsk_images = new Dictionary<ulong, sk_image>();


        [Command("Protocol 34-D", RunMode=RunMode.Async), Alias("r34")]
        public async Task r34_image(params string[] Args)
        {
            string result = p.callers.AskRequest("http://rule34.xxx/index.php?page=dapi&s=post&q=index&limit=1&id=" + p.rand.Next(100000));
            string extention;

            lastr_image = new r34_image(result);
            extention = lastr_image._file_url.Split('.')[lastr_image._file_url.Split('.').Length - 1];
            if (!Directory.Exists("Ressources"))
            {
                Directory.CreateDirectory("Ressources");
            }
            p.callers.DownloadRessource(lastr_image._file_url, "Ressources/NewrImage." + extention);
            await Context.Channel.SendFileAsync("Ressources/NewrImage." + extention);
            File.Delete("Ressources/NewrImage." + extention);
        }

        [Command("Sankaku Complex", RunMode=RunMode.Async)]
        public async Task sankaComplex(params string[] Args)
        {
            int page = p.rand.Next(1, 25);
            ulong userId = Context.User.Id;

            string builder = p.callers.GetSakuraComplexResponse("https://capi-beta.sankakucomplex.com/post/index.json?page=" + page + "&limit=1");
            builder = builder.Replace("\\u0026", "&");
            if (lastsk_images != null)
            {
                if (lastsk_images.ContainsKey(userId))
                {
                    lastsk_images[userId] = new sk_image(builder);
                }
                else
                {
                    lastsk_images.Add(userId, new sk_image(builder));
                }
            }
            p.callers.DownloadRessource_fromSankakuComplex(lastsk_images[userId]._file_furl, "Ressources/" + lastsk_images[userId]._name);
            await Context.Channel.SendFileAsync("Ressources/" + lastsk_images[userId]._name);
            File.Delete("Ressources/" + lastsk_images[userId]._name);
        }

        [Command("Get last skimage")]
        public async Task get_iminfos(params string[] Args)
        {
            ulong userId = Context.User.Id;

            if (Args.Length < 1)
            {
                await ReplyAsync("Il faudrait que je sache ce que vous voulez..");
            }
            if (lastsk_images.ContainsKey(userId) == false)
            {
                await ReplyAsync("Vous n'avez pas encore demandé d'image cette section");
                return;
            }
            if (Args[0] == "infos")
            {
                await ReplyAsync("", false, Speetch.build_skimageinfos(lastsk_images[userId]).Build());
                return;
            }
        }

        [Command("Get last r image")]
        public async Task r_findInfos(params string[] Args)
        {
            if (lastr_image == null)
            {
                await ReplyAsync("Je n'ai pas encore envoyé d'image depuis la dernière fois..");
                return;
            }
            if (Args[0] == "rating")
            {
                await ReplyAsync("La dernière image était rated " + lastr_image._rating);
            }
            else if (Args[0] == "url")
            {
                await ReplyAsync("Voici une url vers la derière image : " + lastr_image._file_url);
            }
            else if (Args[0] == "source")
            {
                if (lastr_image._source == "")
                {
                    await ReplyAsync("La source de cette image n'était pas spécifiée");
                    return;
                }
                else
                    await ReplyAsync("La dernière image provenait de " + lastr_image._source);
            }
            else if (Args[0] == "lolicon")
            {
                if (lastr_image._isLoli == true)
                {
                    await ReplyAsync("Il y avait au moins une loli sur la dernière image");
                    return;
                }
                else
                    await ReplyAsync("Il n'y avait pas de loli sur la dernière image");
            }
            else if (Args[0] == "tags")
            {
                await ReplyAsync("Les tags de la derière image étaient : ");
                await ReplyAsync(Program.makeArgs(lastr_image._tags));
            }
            else
                await ReplyAsync("Je ne vois pas vraiment ce que vous voulez me demander");
        }
    }
}
