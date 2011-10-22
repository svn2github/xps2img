using System;

namespace Xps2ImgUI.Model
{
    public class ConvertionProgressEventArgs: EventArgs
    {
        public readonly int Percent;
        public readonly string Pages;
        public readonly string File;

        public ConvertionProgressEventArgs(int percent, string pages, string file)
        {
            Percent = percent;
            Pages = pages;
            File = file;
        }
    }
}
