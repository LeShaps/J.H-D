using System.Globalization;

namespace J.H_D.Data.Extensions
{
    public static class FormatUtilities
    {
        public static string TrailingCharacters(this int nb, char character, int size)
        {
            string nbString = nb.ToString(CultureInfo.InvariantCulture);
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
