using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Xps2Img.Shared.CommandLine;
using Xps2Img.Shared.TypeConverters;

namespace Xps2Img.Shared.TypeEditors
{
    public class CpuAffinityUITypeEditor : CheckedListBoxUITypeEditor<string>
    {
        private static IEnumerable<CheckItem> GetAll(bool check = true)
        {
            return ProcessorsNumberTypeConverter.Processors.Skip(1).Select(x => new CheckItem { Item = (int.Parse(x) - 1).ToString(CultureInfo.InvariantCulture), Checked = check });
        }

        protected CpuAffinityUITypeEditor()
        {
            _items = GetAll();
        }

        protected override object DefaultValue
        {
            get { return Validation.AutoValue; }
        }
        
        protected override object Value
        {
            set { _items = ParseValue(_typeConverter.ConvertToInvariantString(value)); }
            get
            {
                return
                    _typeConverter.ConvertFromInvariantString(
                    CheckItems.All(x => x.Checked)
                        ? (string)DefaultValue
                        : CheckItems.All(x => !x.Checked)
                            ? GetAll().ElementAt(0).ToString()
                            : CheckItems.Aggregate("", (s, x) => x.Checked ? s + (s == "" ? "" : ",") + x.Item : s));
            }
        }

        private readonly CpuAffinityTypeConverter _typeConverter = new CpuAffinityTypeConverter();

        private static IEnumerable<CheckItem> ParseValue(string value)
        {
            if (value != null && value.ToLowerInvariant() == Validation.AutoValue.ToLowerInvariant())
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
