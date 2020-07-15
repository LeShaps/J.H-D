using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J.H_D.Minions
{
    public struct FeatureRequest<T, U>
        where U : Enum
    {
        public FeatureRequest(T answer, U error)
        {
            Answer = answer;
            Error = error;
        }

        public T Answer;
        public U Error;
    }
}
