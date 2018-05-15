using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D
{
    class FcThread
    {
        public uint _trId { private set; get; }
        public string _comm { private set; get; }
        public string _imagesInfos { private set; get; }
        public uint _fsize { private set; get; }
        public string _chan { private set; get; }
        public string _name { private set; get; }

        public FcThread fill_thread(string source)
        {
            _trId = Convert.ToUInt32(Program.getInfos("\"no\":", source, ','));
            _comm = Program.getInfos("\"com\":\"", source, ',');
            _imagesInfos = Program.getInfos("\"tim\":", source, ',') + Program.getInfos("\"ext\":\"", source, '"');
            _fsize = Convert.ToUInt32(Program.getInfos("\"fsize\":", source, ','));
            _name = Program.getInfos("\"name\":\"", source, '"');

            if (_comm != null)
                _comm = Program.removeSymbols(_comm.Substring(0, _comm.Length - 1));
            return (this);
        }

        public void setchan(string chan)
        {
            _chan = chan;
        }
    }

    class ForumsModule : ModuleBase
    {
        Program p = Program.p;
        static FcThread last_fcimage;

        [Command("[Debug] Display last fcimage infos")]
        public async Task displayInfos(params string[] Args)
        {
            Console.WriteLine(last_fcimage._chan);
            Console.WriteLine(last_fcimage._fsize);
            Console.WriteLine(last_fcimage._comm);
            Console.WriteLine(last_fcimage._trId);
            await ReplyAsync("done");
        }

        [Command("Last fcimage context")]
        public async Task GiveContext(params string[] Args)
        {
            if (last_fcimage == null)
            {
                await ReplyAsync("Je n'ai pas encore envoyé d'image venant de 4chan..");
                return;
            }
            else if (last_fcimage._comm == null)
            {
                await ReplyAsync("Désolé, mais je n'ai pas de contexte pour cette image..");
                return;
            }
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = last_fcimage._name,
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Comment",
                        Value = last_fcimage._comm,
                        IsInline = false,
                    }
                },
                ImageUrl = "http://i.4cdn.org/" + last_fcimage._chan + "/" + last_fcimage._imagesInfos,
                Color = Color.DarkGreen,
            };
            await ReplyAsync("Voilà un peu plus de contexte : ", false, embed.Build());
        }

        [Command("4nsfw", RunMode = RunMode.Async)]
        public async Task Fchan_img(params string[] Args)
        {
            try
            {
                List<FcThread> ThreadList = new List<FcThread>();
                string result;
                string[] splitedResult = null;
                string[] chansList = new string[] { "a", "c", "w", "m", "cgl", "cm", "f", "n", "jp", "v", "vg", "vp", "vr", "co", "g", "tv", "k", "o", "an", "tg", "sp", "asp", "sci", "his", "int", "out", "toy", "i", "po", "p", "ck",
                "ic", "wg", "lit", "mu", "fa", "3", "gd", "diy", "wsg", "qst", "biz", "trv", "fit", "x", "adv", "lgbt", "mlp", "news", "wsr", "vip", "b", "r9k", "pol", "bant", "soc", "s4s", "s", "hc", "hm", "h", "u",
                "d", "y", "t", "hr", "gif", "aco", "r"};

                if (Args.Length < 1)
                {
                    string filepath = fchan_rand();
                    await ReplyAsync("Cette image viens de " + last_fcimage._chan);
                    await Context.Channel.SendFileAsync(filepath);
                    File.Delete("Ressources/images4chan" + last_fcimage._chan + last_fcimage._imagesInfos);
                    return;
                }
                if (Args[0] == "Help" || Args[0] == "help")
                {
                    await ReplyAsync("", false, Speetch.FchanHelp);
                    return;
                }
                else
                {
                    bool isExist = false;
                    foreach (string s in chansList)
                    {
                        if (s == Args[0])
                            isExist = true;
                    }
                    if (isExist == false)
                    {
                        await ReplyAsync(Speetch.FcUnknownChan);
                        return;
                    }
                    string filepath = fchan_search(Args[0]);
                    last_fcimage.setchan(Args[0]);
                    await Context.Channel.SendFileAsync(filepath);
                    File.Delete(filepath);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                await Fchan_img(Args);
            }
        }

        private string fchan_search(string chan)
        {
            try
            {
                List<FcThread> ThreadList = new List<FcThread>();
                string result;
                string[] splitedResult = null;
                string[] chansList = new string[] { "a", "c", "w", "m", "cgl", "cm", "f", "n", "jp", "v", "vg", "vp", "vr", "co", "g", "tv", "k", "o", "an", "tg", "sp", "asp", "sci", "his", "int", "out", "toy", "i", "po", "p", "ck",
                "ic", "wg", "lit", "mu", "fa", "3", "gd", "diy", "wsg", "qst", "biz", "trv", "fit", "x", "adv", "lgbt", "mlp", "news", "wsr", "vip", "b", "r9k", "pol", "bant", "soc", "s4s", "s", "hc", "hm", "h", "u",
                "d", "y", "t", "hr", "gif", "aco", "r"};

                result = p.callers.SimpleAsk("http://a.4cdn.org/" + chan + "/catalog.json");
                splitedResult = result.Split(new string[] { "},{" }, StringSplitOptions.None);
                for (int i = 0; i < splitedResult.Length - 1; i++)
                {
                    FcThread currThread = new FcThread().fill_thread(splitedResult[i]);
                    if (currThread._imagesInfos == null || currThread._imagesInfos == "")
                        continue;
                    ThreadList.Add(currThread);
                }
                int randUse = p.rand.Next(ThreadList.Count - 1);
                last_fcimage = ThreadList[randUse];
                if (!Directory.Exists("Ressources"))
                {
                    Directory.CreateDirectory("Ressources");
                }
                p.callers.DownloadRessource("http://i.4cdn.org/" + chan + "/" + last_fcimage._imagesInfos, "Ressources/images4chan" + chan + last_fcimage._imagesInfos);
                return ("Ressources/images4chan" + chan + last_fcimage._imagesInfos);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return (fchan_search(chan));
            }
        }

        private string fchan_rand()
        {
            try
            {
                List<FcThread> ThreadList = new List<FcThread>();
                string result;
                string[] splitedResult = null;
                string[] chansList = new string[] { "a", "c", "w", "m", "cgl", "cm", "f", "n", "jp", "v", "vg", "vp", "vr", "co", "g", "tv", "k", "o", "an", "tg", "sp", "asp", "sci", "his", "int", "out", "toy", "i", "po", "p", "ck",
                "ic", "wg", "lit", "mu", "fa", "3", "gd", "diy", "wsg", "qst", "biz", "trv", "fit", "x", "adv", "lgbt", "mlp", "news", "wsr", "vip", "b", "r9k", "pol", "bant", "soc", "s4s", "s", "hc", "hm", "h", "u",
                "d", "y", "t", "hr", "gif", "aco", "r"};

                string chan = chansList[p.rand.Next(chansList.Length - 1)];
                result = p.callers.SimpleAsk("http://a.4cdn.org/" + chan + "/catalog.json");
                splitedResult = result.Split(new string[] { "},{" }, StringSplitOptions.None);

                for (int i = 0; i < splitedResult.Length - 1; i++)
                {
                    FcThread currThread = new FcThread().fill_thread(splitedResult[i]);
                    if (currThread._imagesInfos == null || currThread._imagesInfos == "")
                        continue;
                    ThreadList.Add(currThread);
                }
                int randUse = p.rand.Next(ThreadList.Count - 1);
                last_fcimage = ThreadList[randUse];
                if (!Directory.Exists("Ressources"))
                {
                    Directory.CreateDirectory("Ressources");
                }
                p.callers.DownloadRessource("http://i.4cdn.org/" + chan + "/" + last_fcimage._imagesInfos, "Ressources/images4chan" + chan + last_fcimage._imagesInfos);
                last_fcimage.setchan(chan);
                return ("Ressources/images4chan" + chan + last_fcimage._imagesInfos);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return (fchan_rand());
            }
        }
    }
}
