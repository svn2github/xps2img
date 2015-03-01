using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xps2Img.Shared.CommandLine
{
    public partial class Options
    {
        [Browsable(false)]
        public List<Interval> SafePages
        {
            get { return Pages ?? new List<Interval> { new Interval(begin: 1) }; }
        }

        [Browsable(false)]       
        public int SafeProcessorsNumber
        {
            get { return ProcessorsNumber == Defaults.Processors ? Environment.ProcessorCount : ProcessorsNumber; }
        }
    }
}
