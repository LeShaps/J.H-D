using J.H_D.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using RethinkDb.Driver.Ast;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Minions.Responses
{
    public class FChanMinion
    {
        public enum RequestType
        {
            Image,
            Thread
        }

        public class RequestOptions
        {
            public string MandatoryWord; // Will be used with the mandatory word update
            public RequestType RequestType;
            public bool AllowNsfw;
        }

        public static async Task<List<Response.FBoard>> UpdateAvailableChans(bool AllowNsfw = true)
        {
            List<Response.FBoard> AvailbleBoards = new List<Response.FBoard>();

            dynamic Json;
            Json = JsonConvert.DeserializeObject(await Program.p.Asker.GetStringAsync("https://a.4cdn.org/boards.json"));

            if (Json == null)
                return null;

            foreach (dynamic item in (JArray)Json["boards"])
            {
                AvailbleBoards.Add(new Response.FBoard()
                {
                    Title = item.board,
                    Name = item.title,
                    Description = Utilities.Clarify((string)item.meta_description),
                    Spoilers = item.spoilers != null ? true : false,
                    Nsfw = item.ws_board == "0" ? true : false
                });
            }

            if (AllowNsfw == false)
                return AvailbleBoards.Where(x => x.Nsfw == false).ToList();
            return AvailbleBoards;
        }

        public static async Task<FeatureRequest<Response.FThread, Error.FChan>> GetRandomThreadFrom(string board, RequestOptions Options)
        {
            List<Response.FBoard> Boards = await UpdateAvailableChans();
            Response.FBoard UsableBoard = null;
            List<Response.FThread> ThreadsList = new List<Response.FThread>();

            if (board != null)
            {
                if (Boards.Any(x => x.Name == board || x.Title == board))
                {
                    UsableBoard = Boards.Where(x => x.Name == board || x.Title == board).First();
                    if (Options.AllowNsfw == false && UsableBoard.Nsfw == true)
                        return new FeatureRequest<Response.FThread, Error.FChan>(null, Error.FChan.Nsfw);
                }
                else
                    return new FeatureRequest<Response.FThread, Error.FChan>(null, Error.FChan.Unavailable);
            }
            else
            {
                if (Options.AllowNsfw == false)
                {
                    List<Response.FBoard> SafeBoards = Boards.Where(x => x.Nsfw == false).ToList();
                    UsableBoard = SafeBoards[Program.p.rand.Next(SafeBoards.Count)];
                }
                else
                    UsableBoard = Boards[Program.p.rand.Next(Boards.Count)];
            }

            board = UsableBoard.Title;
            dynamic InitialJson;

            InitialJson = JsonConvert.DeserializeObject(await Program.p.Asker.GetStringAsync($"https://a.4cdn.org/{UsableBoard.Title}/catalog.json"));

            foreach(dynamic item in (JArray)InitialJson)
            {
                foreach (dynamic Thread in (JArray)item.threads)
                {
                    ThreadsList.Add(new Response.FThread()
                    {
                        Filename = Thread.filename,
                        Extension = Thread.ext,
                        ThreadId = Thread.no,
                        Comm = Utilities.Clarify((string)Thread.comm),
                        From = Thread.name,
                        Tim = Thread.tim,
                        Chan = board
                    });
                    if ((JArray)Thread.last_replies != null)
                        AddResponseThreads(Thread, ref ThreadsList, board);
                }
            }

            if (Options.RequestType == RequestType.Image)
            {
                List<Response.FThread> ImageThreads = ThreadsList.Where(x => x.Filename != null).ToList();

                return new FeatureRequest<Response.FThread, Error.FChan>(
                    ImageThreads[Program.p.rand.Next(ImageThreads.Count - 1)],
                    Error.FChan.None);
            }
            else
            { 
                return new FeatureRequest<Response.FThread, Error.FChan>(
                    ThreadsList[Program.p.rand.Next(ThreadsList.Count)],
                    Error.FChan.None);
            }
        }

        public static async Task<FeatureRequest<Response.FBoard, Error.FChan>> GetBoardInfo(string[] board)
        {
            if (board.Length == 0)
                return new FeatureRequest<Response.FBoard, Error.FChan>(null, Error.FChan.Unavailable);
            string BoardName = Utilities.MakeArgs(board);
            List<Response.FBoard> Boards = await UpdateAvailableChans();

            foreach (Response.FBoard Board in Boards)
            {
                if (BoardName == Board.Title || BoardName == Board.Name)
                    return new FeatureRequest<Response.FBoard, Error.FChan>(Board, Error.FChan.None);
            }
            return new FeatureRequest<Response.FBoard, Error.FChan>(null, Error.FChan.Unavailable);
        }

        private static void AddResponseThreads(dynamic First, ref List<Response.FThread> ThreadList, string chan)
        {
            foreach (dynamic response in (JArray)First.last_replies)
            {
                ThreadList.Add(new Response.FThread()
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
