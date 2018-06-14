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
    partial class FcThread
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

    partial class ForumsModule : ModuleBase
    {
        Program p = Program.p;
        static Dictionary<ulong, FcThread> last_fcimage = new Dictionary<ulong, FcThread>();
        
        [Command("Last fcimage context")]
        public async Task GiveContext(params string[] Args)
        {
            ulong userId = Context.User.Id;
            if (last_fcimage.ContainsKey(userId) == false)
            {
                await ReplyAsync("Vous n'avez pas encore demandé d'image venant de 4chan..");
                return;
            }
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = last_fcimage[userId]._name,
                Fields = new List<EmbedFieldBuilder>()
                {
                    new EmbedFieldBuilder()
                    {
                        Name = "Comment",
                        Value = last_fcimage[userId]._comm,
                        IsInline = false,
                    }
                },
                ImageUrl = "http://i.4cdn.org/" + last_fcimage[userId]._chan + "/" + last_fcimage[userId]._imagesInfos,
                Color = Color.DarkGreen,
            };
            await ReplyAsync("Voilà un peu plus de contexte : ", false, embed.Build());
        }

        [Command("4nsfw", RunMode = RunMode.Async)]
        public async Task Fchan_img(params string[] Args)
        {
            ulong userId = Context.User.Id;

            List<FcThread> ThreadList = new List<FcThread>();
            string[] chansList = new string[] { "a", "c", "w", "m", "cgl", "cm", "f", "n", "jp", "v", "vg", "vp", "vr", "co", "g", "tv", "k", "o", "an", "tg", "sp", "asp", "sci", "his", "int", "out", "toy", "i", "po", "p", "ck",
                "ic", "wg", "lit", "mu", "fa", "3", "gd", "diy", "wsg", "qst", "biz", "trv", "fit", "x", "adv", "lgbt", "mlp", "news", "wsr", "vip", "b", "r9k", "pol", "bant", "soc", "s4s", "s", "hc", "hm", "h", "u",
                "d", "y", "t", "hr", "gif", "aco", "r"};

            if (Args.Length < 1)
            {
                string filepath = await fchan_rand(userId);
                await ReplyAsync("Cette image viens de " + last_fcimage[userId]._chan);
                await Context.Channel.SendFileAsync(filepath);
                File.Delete("Ressources/images4chan" + last_fcimage[userId]._chan + last_fcimage[userId]._imagesInfos);
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
                string filepath = await fchan_search(Args[0], userId);
                last_fcimage[userId].setchan(Args[0]);
                await ReplyAsync(Speetch.Wait);
                await Context.Channel.SendFileAsync(filepath);
                File.Delete(filepath);
            }
        }

        private async Task fchan_getter(string chan, string result, ulong userId, List<FcThread> ThreadList)
        {
            string[] splitedResult = result.Split(new string[] { "},{" }, StringSplitOptions.None);
            for (int i = 0; i < splitedResult.Length - 1; i++)
            {
                FcThread currThread = new FcThread().fill_thread(splitedResult[i]);
                if (currThread._imagesInfos == null || currThread._imagesInfos == "")
                    continue;
                ThreadList.Add(currThread);
            }
            int randUse = p.rand.Next(ThreadList.Count - 1);
            if (last_fcimage.ContainsKey(userId))
            {
                last_fcimage[userId] = ThreadList[randUse];
            }
            else
            {
                last_fcimage.Add(userId, ThreadList[randUse]);
            }
            if (!Directory.Exists("Ressources"))
            {
                Directory.CreateDirectory("Ressources");
            }
            await p.callers.DownloadRessourceAsync("http://i.4cdn.org/" + chan + "/" + last_fcimage[userId]._imagesInfos, "Ressources/images4chan" + chan + last_fcimage[userId]._imagesInfos);
        }

        private async Task<string> fchan_search(string chan, ulong userId)
        {
            List<FcThread> ThreadList = new List<FcThread>();
            string result;
            string[] chansList = new string[] { "a", "c", "w", "m", "cgl", "cm", "f", "n", "jp", "v", "vg", "vp", "vr", "co", "g", "tv", "k", "o", "an", "tg", "sp", "asp", "sci", "his", "int", "out", "toy", "i", "po", "p", "ck",
                "ic", "wg", "lit", "mu", "fa", "3", "gd", "diy", "wsg", "qst", "biz", "trv", "fit", "x", "adv", "lgbt", "mlp", "news", "wsr", "vip", "b", "r9k", "pol", "bant", "soc", "s4s", "s", "hc", "hm", "h", "u",
                "d", "y", "t", "hr", "gif", "aco", "r"};

            result = await p.callers.SimpleAskAsync("http://a.4cdn.org/" + chan + "/catalog.json");
            Console.WriteLine(result);
            await fchan_getter(chan, result, userId, ThreadList);
            return ("Ressources/images4chan" + chan + last_fcimage[userId]._imagesInfos);
        }

        private async Task<string> fchan_rand(ulong userId)
        {
            List<FcThread> ThreadList = new List<FcThread>();
            string result;
            string[] chansList = new string[] { "a", "c", "w", "m", "cgl", "cm", "f", "n", "jp", "v", "vg", "vp", "vr", "co", "g", "tv", "k", "o", "an", "tg", "sp", "asp", "sci", "his", "int", "out", "toy", "i", "po", "p", "ck",
                "ic", "wg", "lit", "mu", "fa", "3", "gd", "diy", "wsg", "qst", "biz", "trv", "fit", "x", "adv", "lgbt", "mlp", "news", "wsr", "vip", "b", "r9k", "pol", "bant", "soc", "s4s", "s", "hc", "hm", "h", "u",
                "d", "y", "t", "hr", "gif", "aco", "r"};

            string chan = chansList[p.rand.Next(chansList.Length - 1)];
            result = p.callers.SimpleAsk("http://a.4cdn.org/" + chan + "/catalog.json");
            await fchan_getter(chan, result, userId, ThreadList);
            last_fcimage[userId].setchan(chan);
            return ("Ressources/images4chan" + chan + last_fcimage[userId]._imagesInfos);
        }
    }
}
