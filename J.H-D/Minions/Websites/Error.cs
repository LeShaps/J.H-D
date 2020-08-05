using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Data
{
    public static partial class Error
    {
        public enum FChan
        {
            Unavailable,
            ThreadExpired,
            Nsfw,
            None
        }

        public enum InspirationnalError
        {
            None,
            Communication
        }

        public enum Complete
        {
            Help,
            Connection,
            None
        }
    }
}
