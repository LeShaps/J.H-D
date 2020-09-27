using System;
using System.Collections.Generic;
using System.Text;

namespace J.H_D.Data.Exceptions
{
    class GameLostException : Exception
    {
        public GameLostException()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Reason">Why the game has been loosed</param>
        public GameLostException(string Reason)
            : base(Reason)
        { }
    }
}
