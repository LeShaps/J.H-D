using System;
using System.Text.RegularExpressions;

namespace J.H_D.Data
{
    public static partial class Response
    {
        public struct FBoard : IEquatable<FBoard>
        {
            [Embedable("Full Name")]
            private string name;
            [Embedable("Simple Name")]
            private string title;
            [Embedable("Description")]
            private string description;
            [Embedable("Can Spoil")]
            private bool spoilers;
            [Embedable("Is NSFW")]
            private bool nsfw;

            public string Name { get => name; set => name = value; }
            public string Title { get => title; set => title = value; }
            public string Description { get => description; set => description = value; }
            public bool Spoilers { get => spoilers; set => spoilers = value; }
            public bool Nsfw { get => nsfw; set => nsfw = value; }

            public bool Equals(FBoard other)
            {
                return
                    GetHashCode() == other.GetHashCode();
            }
        }

        public struct FThread : IEquatable<FThread>
        {
            [Embedable("FileName")]
            private string filename;
            [Embedable("Extension", false)]
            private string extension;
            [Embedable("ThreadID", false)]
            private uint threadId;
            [Embedable("Commentary")]
            private string comm;
            [Embedable("Author")]
            private string from;
            [Embedable("Tim")]
            private string tim;
            [Embedable("Channel")]
            private string chan;

            public string Filename { get => filename; set => filename = value; }
            public string Extension { get => extension; set => extension = value; }
            public uint ThreadId { get => threadId; set => threadId = value; }
            public string Comm { get => comm; set => comm = value; }
            public string From { get => from; set => from = value; }
            public string Tim { get => tim; set => tim = value; }
            public string Chan { get => chan; set => chan = value; }

            public bool Equals(FThread other)
            {
                return
                    GetHashCode() == other.GetHashCode();
            }
        }

        public class Complete 
        {
            public string Content { get; set; }
        }

        public class SCPReport
        {
            private string imageLink;
            private string imageCaption;
            private bool hasImage = false;
            private string Classs;
            private string description;
            private string confinement;
            private string name;
            private int number;

            public string ImageLink { get => imageLink; set => imageLink = value; }
            public string ImageCaption { get => imageCaption; set => imageCaption = value; }
            public bool HasImage { get => hasImage; set => hasImage = value; }
            public string Class { get => Classs; set => Classs = value; }
            public string Description { get => description; set => description = value; }
            public string Confinement { get => confinement; set => confinement = value; }
            public string Name { get => name; set => name = value; }
            public int Number { get => number; set => number = value; }
        }
    }
}
