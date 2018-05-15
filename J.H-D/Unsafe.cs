using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D
{
    public class Image
    {
        public string _file_url { protected set; get; }
        public string _sample_url { protected set; get; }
        public string _preview_url { protected set; get; }
        public char _rating { protected set; get; }
        public bool _isLoli { protected set; get; }
        public string _source { protected set; get; }
    }

    class r34_image : Image
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

        public r34_image fill_image(string source)
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

            return (this);
        }
    }

    class UnsafeModule : ModuleBase
    {
        Program p = Program.p;
        static r34_image lastr_image;

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
