using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using CommandLine;

namespace Xps2Img.Shared.CommandLine
{
    public partial class Options
    {
        [Option(ShortOptionType.None10, Flags = OptionFlags.Internal)]
        [Browsable(false)]
        public virtual string Internal { get; set; }

        [Browsable(false)]
        public bool HasInternal { get { return !String.IsNullOrEmpty(Internal); } }

        [Browsable(false)]
        public string InternalCancelEventName { get { return GetInternalParamerterAt(0); } }

        [Browsable(false)]
        public string InternalParentAppMutexName { get { return GetInternalParamerterAt(1); } }

        // ReSharper disable once InconsistentNaming
        [Browsable(false)]
        public CultureInfo InternalCulture
        {
            get
            {
                return HasInternal
                         ? CultureInfo.GetCultureInfo(Convert.ToInt32(GetInternalParamerterAt(2), CultureInfo.InvariantCulture))
                         : CultureInfo.InvariantCulture;
            }
        }

        protected const char InternalParametersSeparator = '-';

        private IEnumerable<string> InternalParameters { get { return Internal.Split(InternalParametersSeparator); } }

        private string GetInternalParamerterAt(int index)
        {
            return InternalParameters.Skip(index).First();
        }
    }
}
