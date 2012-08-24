namespace Xps2Img.CommandLine
{
    public static partial class CommandLine
    {
        public enum ReturnCode
        {
            // OK.
            OK = 0,
            // Errors.
            InvalidArg = 1,
            NoArgs = 2,
            Failed = 3,
            InvalidPages = 4,
            InternalOK = -1
        }
    }
}
