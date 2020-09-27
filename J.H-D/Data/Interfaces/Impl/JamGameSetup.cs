using Discord;
using J.H_D.Data.Abstracts;
using J.H_D.Data.Abstracts.Impl;
using J.H_D.Data.Exceptions;
using J.H_D.Minions.Infos;
using System;
using System.Collections.Generic;
using static J.H_D.Data.Response;

namespace J.H_D.Data.Interfaces.Impl
{
    class JamGameSetup : ISetup
    {
        List<Tuple<string, bool>> LoadedLyrics = new List<Tuple<string, bool>>();
        Tuple<SongLyrics?, List<Tuple<string, bool>>> LyricsInfos;

        public AGame CreateGame(IMessageChannel MessageChannel, IUser User)
            => new LetsJam(MessageChannel, User, this, new EmbedMode());

        public string[] GetGameNames()
            => new string[] { "jam", "let's jam" };

        public object GetLoadedParameters()
            => LyricsInfos;

        public string GetNameArg()
            => null;

        public bool HaveVariableArguments()
            => true;

        public IReadOnlyCollection<ISetupResult> Load()
        {
            throw new NotImplementedException();
        }

        public void LoadParameters(string Parameters)
        {
            var Result = MusicMinion.GetLyrics(new[] { Parameters }).Result;

            if (Result.Error == Error.LyricsMatch.NotFound) {
                throw new GameLoadException("Lyrics not found");
            }

            string UsableResults = Result.Answer.Value.Lyrics.Replace(",", "");

            foreach (string Word in UsableResults.Split(" ")) {
                if (!Word.Contains("\n"))
                    LoadedLyrics.Add(new Tuple<string, bool>(Word, false));
                else
                {
                    var split = Word.Split("\n");
                    LoadedLyrics.Add(new Tuple<string, bool>(split[0], false));
                    for (int i = 1; i < split.Length; i++)
                        LoadedLyrics.Add(new Tuple<string, bool>("\n", true));
                    LoadedLyrics.Add(new Tuple<string, bool>(split[split.Length - 1], false));
                }
            }

            LyricsInfos = new Tuple<SongLyrics?, List<Tuple<string, bool>>>(Result.Answer, LoadedLyrics);
        }
    }
}
