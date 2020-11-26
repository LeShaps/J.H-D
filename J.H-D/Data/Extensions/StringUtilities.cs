using System.Collections.Generic;

namespace J.H_D.Data.Extensions
{
    public static class StringUtilities
    {
        public static bool IsNullOrEmpty(this string S)
        {
            if (S == null) return true;
            if (S.Length == 0) return true;

            return false;
        }

        public static string ToWordOnly(this string Word)
        {
            string Result = "";

            if (Word.IsNullOrEmpty())
                return Word;

            foreach (char c in Word)
            {
                if (char.IsLetter(c))
                    Result += c;
            }

            return Result;
        }
        
        public static List<string> CutInParts(this string Content, string Separator, int MaxCharacters)
        {
            List<string> Parts = new List<string>();
            string CurrentPart = "";

            foreach (string Section in Content.Split(Separator))
            {
                if (CurrentPart.Length + Section.Length + Separator.Length > MaxCharacters) {
                    Parts.Add(CurrentPart);
                    CurrentPart = "";
                }
                CurrentPart += Section + Separator;
            }
            Parts.Add(CurrentPart);

            return Parts;
        }

        public static bool Contains(this string String, params string[] Contained)
        {
            foreach (string s in Contained) {
                if (!String.Contains(s)) return false;
            }

            return true;
        }
    }
}
