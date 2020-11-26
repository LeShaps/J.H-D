using System;

namespace J.H_D.Data.Exceptions
{
    public class InvalidGameAnwserException : Exception
    {
        public readonly bool Sneaky;
        public readonly bool ContinueGame;

        public InvalidGameAnwserException(string Message)
            :base(Message)
        {
            Sneaky = false;
            ContinueGame = false;
        }

        public InvalidGameAnwserException(string Message, bool Sneaky)
            :base(Message)
        {
            this.Sneaky = Sneaky;
            ContinueGame = false;
        }

        public InvalidGameAnwserException(string Message, bool Sneaky, bool Continue)
            :base(Message)
        {
            this.Sneaky = Sneaky;
            ContinueGame = Continue;
        }
    }
}
