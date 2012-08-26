namespace Xps2Img.CommandLine
{
    // ReSharper disable PartialTypeWithSinglePart
    public static partial class CommandLine
    {
        public class ReturnCode
        {
            // OK.
            public const int OK             = 0;
            // Errors.
            public const int InvalidArg     = 1;
            public const int NoArgs         = 2;
            public const int Failed         = 3;
            public const int InvalidPages   = 4;
            public const int UserCancelled  = 5;
            public const int InternalOK     = -1;
        }
    }
}
