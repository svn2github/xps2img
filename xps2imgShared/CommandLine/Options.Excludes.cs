﻿using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Xps2Img.Shared.CommandLine
{
    public partial class Options
    {
        public static readonly string[] _excludedOnSave   = { Names.Internal, Names.Batch };

        public static readonly string[] ExcludedUIOptions = { Names.Processors, Names.Batch };
        public static readonly string[] ExcludedOnLaunch  = ExcludedUIOptions.Concat(new[] { ShortOptions.Pages.ToString(CultureInfo.InvariantCulture) }).ToArray();

        public static readonly string[] ExcludeOnResumeCheck =
        {
            Properties.ProcessorsNumber,
            Properties.ProcessPriority,
            Properties.PostAction,
            Properties.IgnoreExisting,
            Properties.IgnoreErrors
        };

        private string ExcludePreferDpiOverSize
        {
            get
            {
                return (PreferDpiOverSize ? ShortOptions.RequiredSize : ShortOptions.Dpi).ToString();
            }
        }

        [Browsable(false)]
        public string[] ExcludedOnSave
        {
            get
            {
                return _excludedOnSave.Concat(new[] { ExcludePreferDpiOverSize }).ToArray();
            }
        }

        [Browsable(false)]
        public string[] ExcludedOnView
        {
            get
            {
                return ExcludedOnSave.Concat(ExcludedUIOptions).ToArray();
            }
        }
    }
}
