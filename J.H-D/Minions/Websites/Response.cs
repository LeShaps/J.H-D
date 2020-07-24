using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Minions.Responses
{
    public static partial class Response
    {
        public class FBoard
        {
            public string Name;
            public string Title;
            public string Description;
            public bool Spoilers;
            public bool Nsfw;
        }

        public class FThread
        {
            public string Filename;
            public string Extension;
            public uint ThreadId;
            public string Comm;
            public string From;
            public string Tim;
            public string Chan;
        }
    }
}
