using System;

namespace J.H_D.Data.Exceptions
{
    class GameLoadException : Exception
    {
        public GameLoadException()
            : base()
        {}

        public GameLoadException(string Message) :
            base(Message)
        {}
    }
}
