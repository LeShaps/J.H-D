using Discord.Commands;
using J.H_D.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using J.H_D.Data.Abstracts;
using J.H_D.Data.Exceptions;

namespace J.H_D.Modules
{
    class GameModule : ModuleBase
    {
        [Help("Game", "Start a game!")]
        [Command("Play")]
        public async Task PlayAsync(string GameName, [Remainder] string mode = null)
        {
            if (JHConfig.Games.Any(x => x.IsMyGame(Context.Channel.Id))) {
                await ReplyAsync("Can't, already do");
                return;
            }
            try
            {
                var Game = LoadGame(GameName.ToLowerInvariant(), Context.Channel, Context.User, mode);
                if (Game == null)
                    await ReplyAsync("Eh, dunno about this one");

                JHConfig.Games.Add(Game);
                await Game.StartAsync();
            }
            catch (Exception e) when (e is GameLoadException)
            {
                await ReplyAsync(e.Message);
            }
        }

        [Command("Cancel", RunMode = RunMode.Async)]
        public async Task CancelAsync()
        {
            var game = JHConfig.Games.Find(x => x.IsMyGame(Context.Channel.Id));
            if (game == null)
            {
                await ReplyAsync("There's no game running in this channel");
                return;
            }
            await game?.CancelAsync();
        }

        public AGame LoadGame(string gameName, IMessageChannel Channel, IUser User, string argument)
        {
            foreach (var Preload in JHConfig.Preloads)
            {
                if (Preload.GetGameNames().Contains(gameName) && (Preload.GetNameArg() == argument && !Preload.HaveVariableArguments()))
                    return Preload.CreateGame(Channel, User);
                else if (Preload.GetGameNames().Contains(gameName) && (Preload.GetNameArg() == argument || Preload.HaveVariableArguments()))
                {
                    try
                    {
                        Preload.LoadParameters(argument);
                    }
                    catch (Exception e) when (e is GameLoadException)
                    {
                        throw e;
                    }
                    return Preload.CreateGame(Channel, User);
                }
            }
            return null;
        }
    }
}
