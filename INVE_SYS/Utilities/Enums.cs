namespace INVE_SYS.Utilities
{
    public class Enums
    {

        public enum ExceptionType
        {
            Error = 1,
            Warning = 2,
            Info = 3
        }

        public static class CommonConditions
        {
            public const int Reserved = 1;
            public const int Cancelled = 2;
        }

        public static class MovementsType
        {
            public const string Entry = "ENTRY";
            public const string Exit = "EXIT";
        }
    }
}
