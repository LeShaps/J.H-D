﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using J.H_D.Tools;
using J.H_D.Data;

using FBoard = J.H_D.Data.Response.FBoard;
using FThread = J.H_D.Data.Response.FThread;

namespace J.H_D.Minions.Websites
{
    public class FChanMinion
    {
        public enum RequestType
        {
            Image,
            Thread
        }

        public struct RequestOptions : IEquatable<RequestOptions>
        {
            public string MandatoryWord { get; set; }
            public RequestType RequestType { get; set; }
            public bool AllowNsfw { get; set; }

            public bool Equals(RequestOptions other)
            {
                return
                    MandatoryWord == other.MandatoryWord &&
                    RequestType == other.RequestType &&
                    AllowNsfw == other.AllowNsfw;
            }
        }

        public static async Task<List<FBoard>> UpdateAvailableChansAsync(bool AllowNsfw = true)
        {
            List<FBoard> AvailbleBoards = new List<FBoard>();

            dynamic Json;
            Json = JsonConvert.DeserializeObject(await Program.p.Asker.GetStringAsync("https://a.4cdn.org/boards.json"));

            if (Json == null)
                return null;

            foreach (dynamic item in (JArray)Json["boards"])
            {
                AvailbleBoards.Add(new FBoard
                {
                    Title = item.board,
                    Name = item.title,
                    Description = Utilities.Clarify((string)item.meta_description),
                    Spoilers = item.spoilers != null ? true : false,
                    Nsfw = item.ws_board == "0"
                });
            }

            if (!AllowNsfw)
                return AvailbleBoards.Where(x => !x.Nsfw).ToList();
            return AvailbleBoards;
        }

        public static async Task<FeatureRequest<FThread?, Error.FChan>> GetRandomThreadFromAsync(string board, RequestOptions Options)
        {
            List<FBoard> Boards = await UpdateAvailableChansAsync().ConfigureAwait(false);
            FBoard UsableBoard;
            List<FThread> ThreadsList = new List<FThread>();

            if (board != null)
            {
                if (Boards.Any(x => x.Name == board || x.Title == board))
                {
                    UsableBoard = Boards.Where(x => x.Name == board || x.Title == board).First();
                    if (!Options.AllowNsfw && UsableBoard.Nsfw) {
                        return new FeatureRequest<FThread?, Error.FChan>(null, Error.FChan.Nsfw);
                    }
                } else {
                    return new FeatureRequest<FThread?, Error.FChan>(null, Error.FChan.Unavailable);
                }
            } else {
                if (!Options.AllowNsfw) {
                    List<FBoard> SafeBoards = Boards.Where(x => !x.Nsfw).ToList();
                    UsableBoard = SafeBoards[Program.p.rand.Next(SafeBoards.Count)];
                } else {
                    UsableBoard = Boards[Program.p.rand.Next(Boards.Count)];
                }
            }

            board = UsableBoard.Title;
            dynamic InitialJson;

            InitialJson = JsonConvert.DeserializeObject(await Program.p.Asker.GetStringAsync($"https://a.4cdn.org/{UsableBoard.Title}/catalog.json"));

            foreach (dynamic item in (JArray)InitialJson)
            {
                foreach (dynamic Thread in (JArray)item.threads)
                {
                    ThreadsList.Add(new FThread
                    {
                        Filename = Thread.filename,
                        Extension = Thread.ext,
                        ThreadId = Thread.no,
                        Comm = Utilities.Clarify((string)Thread.comm),
                        From = Thread.name,
                        Tim = Thread.tim,
                        Chan = board
                    });
                    if ((JArray)Thread.last_replies != null) {
                        AddResponseThreads(Thread, ref ThreadsList, board);
                    }
                }

                if (Options.RequestType == RequestType.Image)
                {
                    List<FThread> ImageThreads = ThreadsList.Where(x => x.Filename != null).ToList();

                    return new FeatureRequest<FThread?, Error.FChan>(
                        ImageThreads[Program.p.rand.Next(ImageThreads.Count - 1)],
                        Error.FChan.None);
                } else {
                    return new FeatureRequest<FThread?, Error.FChan>(
                        ThreadsList[Program.p.rand.Next(ThreadsList.Count)],
                        Error.FChan.None);
                }
            }

            return new FeatureRequest<FThread?, Error.FChan>(null, Error.FChan.Unavailable);
        }

        public static async Task<FeatureRequest<FBoard?, Error.FChan>> GetBoardInfoAsync(string[] board)
        {
            if (board.Length == 0) {
                return new FeatureRequest<FBoard?, Error.FChan>(null, Error.FChan.Unavailable);
            }

            string BoardName = Utilities.MakeArgs(board);
            List<FBoard> Boards = await UpdateAvailableChansAsync().ConfigureAwait(false);

            foreach (FBoard Board in Boards)
            {
                if (BoardName == Board.Title || BoardName == Board.Name) {
                    return new FeatureRequest<FBoard?, Error.FChan>(Board, Error.FChan.None);
                }
            }

            return new FeatureRequest<FBoard?, Error.FChan>(null, Error.FChan.Unavailable);
        }

        private static void AddResponseThreads(dynamic First, ref List<FThread> ThreadList, string chan)
        {
            foreach (dynamic response in (JArray)First.last_replies)
            {
                ThreadList.Add(new FThread
                {
                    Filename = response.filename,
                    Extension = response.ext,
                    ThreadId = response.no,
                    Comm = Utilities.Clarify((string)response.com),
                    From = response.name,
                    Tim = response.tim,
                    Chan = chan
                });
            }
        }
    }
}
