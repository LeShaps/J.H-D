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
    }
}
