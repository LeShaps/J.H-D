using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Minions.Responses
{
    public static partial class Error
    {
        public enum Movie
        {
            None,
            NotFound,
            Help,
        }

        public enum Urban
        {
            WordNotFound,
            None
        }

        public enum Brainz
        {
            ArtistNotFound,
            TitleNotFound,
            ConnectionError,
            None
        }
    }
}
