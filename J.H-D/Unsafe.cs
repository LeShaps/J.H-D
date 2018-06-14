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
    class UnsafeModule : ModuleBase
    {
        Program p = Program.p;
        static r34_image lastr_image;
        static Dictionary<ulong, sk_image> lastsk_images = new Dictionary<ulong, sk_image>();
        static Dictionary<ulong, dan_image> lastdan_images = new Dictionary<ulong, dan_image>();
        static Dictionary<ulong, kon_image> lastkon_images = new Dictionary<ulong, kon_image>();
        
        [Command("Protocol 34-D", RunMode=RunMode.Async), Alias("r34")]
        public async Task r34_image(params string[] Args)
        {
            string result = await p.callers.AskRequestAsync("http://rule34.xxx/index.php?page=dapi&s=post&q=index&limit=1&id=" + p.rand.Next(100000));
            string extention;

            lastr_image = new r34_image(result);
            extention = lastr_image._file_url.Split('.')[lastr_image._file_url.Split('.').Length - 1];
            if (!Directory.Exists("Ressources"))
            {
                Directory.CreateDirectory("Ressources");
            }
            await p.callers.DownloadRessourceAsync(lastr_image._file_url, "Ressources/NewrImage." + extention);
            await Context.Channel.SendFileAsync("Ressources/NewrImage." + extention);
            File.Delete("Ressources/NewrImage." + extention);
        }
        
        private int GetmaxTags(string tags)
        {
                return (Convert.ToInt32(Program.getInfos("posts count=\"", p.callers.AskRequest("https://www.konachan.com/post.xml?limit=1&tags=" + tags), '"')));
        }

        [Command("Konachan", RunMode=RunMode.Async)]
        public async Task AskKonachan(params string[] Args)
        {
            ulong userId = Context.User.Id;
            int page = p.rand.Next(100000);
            string baseurl = "https://konachan.com/post.xml?limit=1";
            if (Args.Length >= 1)
            {
                string asks = Program.makeArgs(Args).Replace(' ', '+');
                page = p.rand.Next(GetmaxTags(asks));
                baseurl += "&tags=" + asks;
            }
            baseurl += "&page=" + page;
            string result = await p.callers.AskRequestAsync(baseurl);
            if (result == null)
            {
                await ReplyAsync("Quelque chose s'est mal passé");
                return;
            }
            if (lastkon_images.ContainsKey(userId))
            {
                lastkon_images[userId] = new kon_image(result);
            }
            else
            {
                lastkon_images.Add(userId, new kon_image(result));
            }
            Program.checkDir("Ressources");
            await p.callers.DownloadRessourceAsync(lastkon_images[userId]._file_url, "Ressources/" + lastkon_images[userId]._name);
            await ReplyAsync(Speetch.Wait);
            await Context.Channel.SendFileAsync("Ressources/" + lastkon_images[userId]._name);
            File.Delete("Ressources/" + lastkon_images[userId]._name);
        }

        [Command("Danbooru", RunMode=RunMode.Async)]
        public async Task Askdanbooru(params string[] Args)
        {
            ulong userId = Context.User.Id;
            int page = p.rand.Next(10000);
            string login = File.ReadAllLines("Logers/danboorulogins.txt")[0];
            string mdp = File.ReadAllLines("Logers/danboorulogins.txt")[1];
            string baseurl = "https://danbooru.donmai.us/posts.xml?limit=1&random=true";
            if (Args.Length > 1)
            {
                string args = Program.makeArgs(Args).Replace(' ', '+');
                baseurl += "&tags=" + args + "&page=1";
            }
            else
            {
                baseurl += "&page=" + page;
            }
            string requestresult = await p.callers.AskWithCredentialsAsync(baseurl + "/posts/" + page + ".xml", login, mdp);
            if (requestresult == null)
            {
                await ReplyAsync("Quelque chose s'est mal passé");
            }
            if (lastdan_images.ContainsKey(userId))
            {
                lastdan_images[userId] = new dan_image(requestresult);
            }
            else
            {
                lastdan_images.Add(userId, new dan_image(requestresult));
            }
            await p.callers.DownloadRessourceAsync(lastdan_images[userId]._file_url, "Ressources/" + lastdan_images[userId]._name);
            await ReplyAsync(Speetch.Wait);
            await Context.Channel.SendFileAsync("Ressources/" + lastdan_images[userId]._name);
            File.Delete("Ressources/" + lastdan_images[userId]._name);
        }

        [Command("Fav last danimage")]
        public async Task fav_danimage(params string[] Args)
        {
            ulong userId = Context.User.Id;
            foreach (KeyValuePair<ulong, dan_image> d in lastdan_images)
            {
                Console.WriteLine("Id : " + d.Key);
            }

            if (lastdan_images.ContainsKey(userId) == false)
            {
                await ReplyAsync("Vous n'avez pas encore demandé d'image venant de DanBooru cette section");
            }
            else
            {
                Program.checkDir("Users/" + userId.ToString() + "/Fav/DanBooruImages");
                File.AppendAllText("Users/" + userId.ToString() + "/Fav/DanBooruImages/" + "favedimages.saved", lastdan_images[userId].ToString());
                await ReplyAsync("Cette image à bien été ajoutée à vos favoris");
            }
        }

        [Command("Fav last skimage")]
        public async Task fav_skimage(params string[] Args)
        {
            ulong userId = Context.User.Id;
            foreach (KeyValuePair<ulong, sk_image> d in lastsk_images)
            {
                Console.WriteLine("Id : " + d.Key);
            }

            if (lastsk_images.ContainsKey(userId) == false)
            {
                await ReplyAsync("Vous n'avez pas encore demandé d'image venant de Sankaku Complex cette section");
            }
            else
            {
                Program.checkDir("Users/" + userId.ToString() + "/Fav/SankakuComplexImages");
                File.AppendAllText("Users/" + userId.ToString() + "/Fav/SankakuComplexImages/" + "favedimages.saved", lastsk_images[userId].ToString());
                await ReplyAsync("Cette image à bien été ajoutée à vos favoris");
            }
        }

        [Command("Fav last konimage")]
        public async Task fav_koimage(params string[] Args)
        {
            ulong userId = Context.User.Id;

            if (lastkon_images.ContainsKey(userId) == false)
            {
                await ReplyAsync("Vous n'avez pas encore demandé d'image venant de Konachan cette section");
            }
            else
            {
                Program.checkDir("Users/" + userId.ToString() + "/Fav/KonachanImages");
                File.AppendAllText("Users/" + userId.ToString() + "/Fav/KonachanImages/" + "favedimages.saved", lastkon_images[userId].ToString());
                await ReplyAsync("Cette image à été rajoutée à vos favoris");
            }
        }

        [Command("Sankaku Complex", RunMode=RunMode.Async)]
        public async Task sankaComplex(params string[] Args)
        {
            int page = p.rand.Next(1, 25);
            ulong userId = Context.User.Id;
            
            string builder = await p.callers.AskWithCredentialsAsync("https://capi-beta.sankakucomplex.com/post/index.json?page=" + page + "&limit=1", "JH", "nooneknows");
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
            await p.callers.DownloadRessourceAsync(lastsk_images[userId]._file_furl, "Ressources/" + lastsk_images[userId]._name);
            await ReplyAsync(Speetch.Wait);
            await Context.Channel.SendFileAsync("Ressources/" + lastsk_images[userId]._name);
            File.Delete("Ressources/" + lastsk_images[userId]._name);
        }

        [Command("Get last konimage")]
        public async Task get_koninfos(params string[] args)
        {
            ulong userId = Context.User.Id;
            string pargs = Program.makeArgs(args);

            if (args.Length < 1)
            {
                await ReplyAsync("Il faudrait que je sache ce que vous voulez..");
                return;
            }
            if (lastkon_images.ContainsKey(userId) == false)
            {
                await ReplyAsync("Vous n'avez pas encore demadné d'image cette section");
                return;
            }
            if (pargs == "infos")
            {
                await Context.Channel.SendMessageAsync("", false, Speetch.konbuilderwtag(lastkon_images[userId]).Build());
                return;
            }
            if (pargs == "infos with tags")
            {
                await Context.Channel.SendMessageAsync("", false, Speetch.konbuilder(lastkon_images[userId]).Build());
            }
        }

        [Command("Get last danimage")]
        public async Task get_daninfos(params string[] args)
        {
            ulong userId = Context.User.Id;
            string pargs = Program.makeArgs(args);

            if (args.Length < 1)
            {
                await ReplyAsync("Il faudrait que je sache ce que vous voulez..");
                return;
            }
            if (lastdan_images.ContainsKey(userId) == false)
            {
                await ReplyAsync("Vous n'avez pas encore demadné d'image cette section");
                return;
            }
            if (pargs == "infos")
            {
                await Context.Channel.SendMessageAsync("", false, Speetch.tembuild_dan(lastdan_images[userId]).Build());
                return;
            }
            if (pargs == "infos with tags")
            {
                await Context.Channel.SendMessageAsync("", false, Speetch.tembuild_danwtag(lastdan_images[userId]).Build());
            }
        }

        [Command("Get last skimage")]
        public async Task get_iminfos(params string[] Args)
        {
            ulong userId = Context.User.Id;

            if (Args.Length < 1)
            {
                await ReplyAsync("Il faudrait que je sache ce que vous voulez..");
                return;
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
