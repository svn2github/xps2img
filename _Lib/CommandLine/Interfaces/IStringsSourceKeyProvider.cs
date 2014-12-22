using System;

namespace CommandLine.Interfaces
{
    public interface IStringsSourceKeyProvider
    {
        string FormatKey(Type type, string id, string idCategory);
    }
}
