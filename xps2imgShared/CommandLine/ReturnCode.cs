namespace Xps2Img.Shared.CommandLine
{
    // ReSharper disable PartialTypeWithSinglePart
    public static partial class CommandLine
    {
        public static class ReturnCode
        {
            // OK.
            public const int OK                 = 0;

            // Errors.
            public const int InvalidArg         = 1;
            public const int NoArgs             = 2;
            public const int Failed             = 3;
            public const int InvalidPages       = 4;
            public const int UserCancelled      = 5;

            // Internals.
            public const int InternalOK         = 0xDEADBEE;
        }
    }
}
