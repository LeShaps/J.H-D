using System;
using System.Collections.Generic;
using System.Text;

namespace J.H_D.Data.Extensions
{
    public static class FormatUtilities
    {
        public static string TrailingCharacters(this int nb, char character, int size)
        {
            string nbString = nb.ToString();
            string NewStringNb = "";
            if (nbString.Length >= size)
                return nbString;
            else
            {
                for (int i = 0; i < size - nbString.Length; i++)
                    NewStringNb += character;
                NewStringNb += nbString;
                return NewStringNb;
            }
        }
    }
}
