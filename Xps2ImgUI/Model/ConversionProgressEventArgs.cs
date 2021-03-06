﻿using System;

namespace Xps2ImgUI.Model
{
    public class ConversionProgressEventArgs: EventArgs
    {
        public double Percent { get; private set; }
        public int Page { get; private set; }
        public string Pages { get; private set; }
        public string File { get; private set; }

        public ConversionProgressEventArgs(double percent, int page, string pages, string file)
        {
            Percent = percent;
            Page = page;
            Pages = pages;
            File = file;
        }
    }
}
