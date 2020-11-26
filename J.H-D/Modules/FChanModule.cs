using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using J.H_D.Minions.Websites;
using J.H_D.Minions;
using J.H_D.Tools;
using J.H_D.Data;

namespace J.H_D.Modules
{
    class FChanModule : ModuleBase
    {
        [Command("FChan available boards"), Alias("4chan boards")]
        [Help("4chan", "Display a list of 4chan boards available")]
        public async Task DisplayFchanBoardsAsync()
        {
            await Program.DoActionAsync(Context.User, Context.Message.Id, Module.Forum);

            List<Response.FBoard> Boards = await FChanMinion.UpdateAvailableChansAsync(true);
            await ReplyAsync("", false, BoardInfosBuilder(Boards));
        }

        [Command("Fchan board info"), Alias("4chan board info")]
        [Help("4chan", "Get a few more infos about a 4chan board")]
        [Parameter("Board", "The board you want infos on, it can either be the / name, of the complete name", ParameterType.Mandatory)]
        public async Task GetBoardInfosAsync(params string[] Args)
        {
            await Program.DoActionAsync(Context.User, Context.Message.Id, Module.Forum);

            var result = await FChanMinion.GetBoardInfoAsync(Args);

            switch (result.Error)
            {
                case Error.FChan.Unavailable:
                    await ReplyAsync("The chan you're looking for is unavailable or doesn't exist");
                    break;

                case Error.FChan.ThreadExpired:
                    await ReplyAsync("The thread you're looking for has expired");
                    break;

                case Error.FChan.None:
                    await ReplyAsync("", false, BoardInfos((Response.FBoard)result.Answer));
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        [Command("Random 4image", RunMode = RunMode.Async), Alias("4chan image")]
        [Help("4chan", "Return a random image from 4chan", Warnings.Spoilers | Warnings.NSFW)]
        [Parameter("Board", "The board from which the image is from, NSFW boards are only availables in NSFW channels", ParameterType.Optional)]
        public async Task RandomImageAsync(params string[] Args)
        {
            var result = new FeatureRequest<Response.FThread?, Error.FChan>();
            ITextChannel chan = (ITextChannel)Context.Channel;

            if (Args.Length >= 1)
            {
                string OneArg = Utilities.MakeArgs(Args);
                result = await FChanMinion.GetRandomThreadFromAsync(OneArg, new FChanMinion.RequestOptions
                {
                    MandatoryWord = null,
                    AllowNsfw = chan.IsNsfw,
                    RequestType = FChanMinion.RequestType.Image
                });
            }
            else
                result = await FChanMinion.GetRandomThreadFromAsync(null, new FChanMinion.RequestOptions
                {
                    MandatoryWord = null,
                    AllowNsfw = chan.IsNsfw,
                    RequestType = FChanMinion.RequestType.Image
                });

            switch (result.Error)
            {
                case Error.FChan.Unavailable:
                    await ReplyAsync("The chan you're looking for is unavailable or doesn't exist");
                    break;

                case Error.FChan.None:
                    await ReplyAsync("", false, ThreadImageBuild((Response.FThread)result.Answer));
                    break;

                case Error.FChan.Nsfw:
                    await ReplyAsync("The board you've asked for is NSFW, please retry in a non-sfw channel");
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        [Command("Tell me")]
        public async Task TellMe([Remainder] string Question)
        {
            ITextChannel chan = (ITextChannel)Context.Channel;

            var Result = await FChanMinion.GetRandomThreadFromAsync(null, new FChanMinion.RequestOptions
            {
                MandatoryWord = null,
                AllowNsfw = chan.IsNsfw,
                RequestType = FChanMinion.RequestType.Image
            });

            await ReplyAsync("", false, ThreadImageBuild((Response.FThread)Result.Answer));
        }

        [Command("Random 4thread"), Alias("Random 4chan thread"), Priority(-1)]
        [Help("4chan", "Return a random thread from 4chan", Warnings.Spoilers | Warnings.NSFW)]
        [Parameter("Board", "The board from which the thread is from, NSFW boards are only availables in NSFW channels", ParameterType.Optional)]
        public async Task RandomThreadAsync(params string[] Args)
        {
            ITextChannel chan = (ITextChannel)Context.Channel;

            var result = new FeatureRequest<Response.FThread?, Error.FChan>();

            if (Args.Length >= 1)
            {
                string OneArg = Utilities.MakeArgs(Args);
                result = await FChanMinion.GetRandomThreadFromAsync(OneArg, new FChanMinion.RequestOptions
                {
                    RequestType = FChanMinion.RequestType.Thread,
                    MandatoryWord = null,
                    AllowNsfw = chan.IsNsfw
                });
            }
            else
                result = await FChanMinion.GetRandomThreadFromAsync(null, new FChanMinion.RequestOptions
                {
                    RequestType = FChanMinion.RequestType.Thread,
                    MandatoryWord = null,
                    AllowNsfw = chan.IsNsfw
                });

            switch (result.Error)
            {
                case Error.FChan.Unavailable:
                    await ReplyAsync("The chan you're looking for is unavailable or doesn't exist");
                    break;

                case Error.FChan.None:
                    await ReplyAsync("", false, ThreadInfosEmbed((Response.FThread)result.Answer));
                    break;

                case Error.FChan.Nsfw:
                    await ReplyAsync("The board you've asked for is NSFW, please retry in a non-sfw channel");
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        private Embed ThreadInfosEmbed(Response.FThread thread)
        {
            EmbedBuilder emb = new EmbedBuilder
            {
                Title = $"From {thread.From} on {thread.Chan}",
                Url = $"http://4chan.org/{thread.Chan}/thread/{thread.ThreadId}",
                Color = Color.DarkGreen,
                Description = Utilities.GetPlainTextFromHtml(thread.Comm)
            };
            if (thread.Filename != null)
            {
                emb.ImageUrl = $"http://i.4cdn.org/{thread.Chan}/{thread.Tim}{thread.Extension}";
                emb.Footer = new EmbedFooterBuilder
                {
                    Text = $"{thread.Filename}{thread.Extension}"
                };
            }
            return emb.Build();
        }

        private Embed ThreadImageBuild(Response.FThread image)
        {
            EmbedBuilder emb = new EmbedBuilder
            {
                Title = $"From {image.From} on {image.Chan}",
                ImageUrl = $"http://i.4cdn.org/{image.Chan}/{image.Tim}{image.Extension}",
                Color = Color.DarkGreen
            };
            return emb.Build();
        }

        private Embed BoardInfos(Response.FBoard Board)
        {
            EmbedBuilder emb = new EmbedBuilder
            {
                Title = Board.Name,
                Url = "https://4chan.org/" + Board.Title,
                Description = Utilities.GetPlainTextFromHtml(Board.Description)
            };
            emb.Color = Color.Green;
            if (Board.Nsfw || Board.Spoilers)
            {
                emb.Color = Color.DarkOrange;
                string WarningPhrase = null;
                if (Board.Nsfw) {
                    WarningPhrase += "- This board contains NSFW content" + Environment.NewLine;
                }
                if (Board.Spoilers) {
                    WarningPhrase += "- This board can contains Spoilers"; 
                }
                emb.AddField("Warning", WarningPhrase);
            }

            return emb.Build();
        }

        private Embed BoardInfosBuilder(List<Response.FBoard> Boards)
        {
            string SafeList = null;
            string NSFWList = null;

            EmbedBuilder emb = new EmbedBuilder
            {
                Title = "4Chan available boards",
                Url = "https://4chan.org/",
                Color = Color.Green
            };

            foreach (Response.FBoard board in Boards)
            {
                if (!board.Nsfw)
                    SafeList = $"{SafeList}{board.Title} - {board.Name}{Environment.NewLine}";
                else
                    NSFWList = $"{NSFWList}{board.Title} - {board.Name}{Environment.NewLine}";
            }

            emb.AddField("Safe chans", SafeList, true);
            emb.AddField("NSFW chans", NSFWList, true);

            return emb.Build();
        }
    }
}
