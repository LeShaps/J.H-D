﻿using Discord;
using System;
using System.Threading.Tasks;
using WebSocketSharp;

using J.H_D.Data;

using Complete = J.H_D.Data.Response.Complete;

namespace J.H_D.Minions.Websites
{
    static class GeneratorMinion
    {
        private const int RefreshDelay = 2000;
        public static async Task<FeatureRequest<Complete, Error.Complete>> CompleteAsync(string Sentence, IUserMessage msg, Func<IUserMessage, string, Task> Updater)
        {
            string Content = Sentence;
            string OldContent = Content;
            bool SendError = false;

            if (Sentence.Length == 0)
                return new FeatureRequest<Complete, Error.Complete>(null, Error.Complete.Help);
            var ws = new WebSocket("ws://163.172.76.10:8080");
            ws.Origin = "http://textsynth.org";

            ws.OnMessage += (sender, e)
                => {
                Content += " " + e.Data.Replace(" .", ".").Replace(" '", "'").Replace(" ,", ",").Replace("*", "\\*").Replace("_", "\\_");
            };

            ws.OnError += (sender, e)
                => {
                SendError = true;
                Content = null;
            };

            ws.OnClose += (sender, e)
                => {
                Content = null;
            };

            ws.Connect();
            ws.Send("g," + Sentence);

            await Task.Run(async () =>
            {
                while (Content != null)
                {
                    OldContent = Content;
                    await Updater(msg, OldContent).ConfigureAwait(false);
                    await Task.Delay(RefreshDelay);
                }
            }).ConfigureAwait(false);

            if (SendError) {
                return new FeatureRequest<Complete, Error.Complete>(null, Error.Complete.Connection);
            }

            return new FeatureRequest<Complete, Error.Complete>(new Complete
            {
                Content = OldContent
            }, Error.Complete.None);
        }
    }
}
