using System;
using System.Collections.Generic;

namespace J.H_D.Data.Extensions
{
    public static class QueryExtensions
    {
        public static List<int> IndexesOf<T>(this List<T> List, T Comparison)
            where T : class
        {
            List<int> Indexes = new List<int>();

            for (int i = 0; i < List.Count; i++)
            {
                if (List[i] == Comparison)
                    Indexes.Add(i);
            }

            return Indexes;
        }

        public static List<int> IndexesOf<T>(this List<T> UList, Func<T, bool> Predicate)
        {
            List<int> Indexes = new List<int>();

            for (int i = 0; i < UList.Count; i++)
            {
                if (Predicate(UList[i]))
                    Indexes.Add(i);
            }

            return Indexes;
        }
    }
}
