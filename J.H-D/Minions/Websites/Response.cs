using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Data
{
    public static partial class Response
    {
        public class FBoard
        {
            [Embedable("Full Name")]
            public string Name;
            [Embedable("Simple Name")]
            public string Title;
            [Embedable("Description")]
            public string Description;
            [Embedable("Can Spoil")]
            public bool Spoilers;
            [Embedable("Is NSFW")]
            public bool Nsfw;
        }

        public class FThread
        {
            [Embedable("FileName")]
            public string Filename;
            [Embedable("Extension", false)]
            public string Extension;
            [Embedable("ThreadID", false)]
            public uint ThreadId;
            [Embedable("Commentary")]
            public string Comm;
            [Embedable("Author")]
            public string From;
            [Embedable("Tim")]
            public string Tim;
            [Embedable("Channel")]
            public string Chan;
        }

        public class Complete 
        {
            public string Content;
        }
    }
}
