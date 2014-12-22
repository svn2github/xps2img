using System;

namespace CommandLine
{
    [Flags]
    public enum OptionFlags
    {
        Internal     = 0x0001,
        Ignore       = 0x0002,
        NoValidation = 0x0004,
        NoDefaultValueDescription = 0x0008
    }
}