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

        public enum SCPError
        {
            Help,
            Reach,
            NonTranslated,
            None
        }

        public enum AbyssError
        {
            Auth,
            DbFail,
            Help,
            None
        }
    }
}
