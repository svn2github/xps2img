using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Xps2Img.CommandLine;
using Xps2ImgUI.Converters;

namespace Xps2ImgUI.TypeEditors
{
    public class CpuAffinityUITypeEditor : CheckedListBoxUITypeEditor<string>
    {
        private static IEnumerable<CheckItem> GetAll(bool check = true)
        {
            return ProcessorsNumberConverter.Processors.Skip(1).Select(x => new CheckItem { Item = (int.Parse(x) - 1).ToString(CultureInfo.InvariantCulture), Checked = check });
        }

        protected CpuAffinityUITypeEditor()
        {
            _items = GetAll();
        }

        protected override string DefaultValue
        {
            get { return Options.AutoValue; }
        }
        
        protected override string Value
        {
            set { _items = ParseValue(value); }
            get
            {
                return
                    CheckItems.All(x => x.Checked)
                        ? DefaultValue
                        : CheckItems.All(x => !x.Checked)
                            ? GetAll().ElementAt(0).ToString()
                            : CheckItems.Aggregate("", (s, x) => x.Checked ? s + (s == "" ? "" : ",") + x.Item : s);
            }
        }

        private static IEnumerable<CheckItem> ParseValue(string value)
        {
            if (value != null && value.ToLowerInvariant() == Options.AutoValue.ToLowerInvariant())
            {
                return GetAll();
            }

            var checkItems = GetAll(false).ToArray();

            if (String.IsNullOrEmpty(value))
            {
                checkItems[0].Checked = true;
                return checkItems;
            }

            var isOneSet = false;
            foreach (var idStr in value.Split(','))
            {
                int id;
                if (int.TryParse(idStr, out id) && id >= 0 && id < checkItems.Length)
                {
                    checkItems[id].Checked = true;
                    isOneSet = true;
                }
            }

            if (!isOneSet)
            {
                return GetAll();
            }

            return checkItems;
        }

        private IEnumerable<CheckItem> _items;

        protected override IEnumerable<CheckItem> CheckItems
        {
            set { _items = value; }
            get { return _items; }
        }
    }
}
