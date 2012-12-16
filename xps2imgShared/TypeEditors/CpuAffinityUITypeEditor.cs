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
        private static IEnumerable<ListItem> NewItems(bool check = true)
        {
            return Enumerable.Range(0, Environment.ProcessorCount).Select(x => new ListItem { Item = x.ToString(CultureInfo.InvariantCulture), Checked = check });
        }

        protected CpuAffinityUITypeEditor()
        {
            _items = NewItems();
        }

        protected override object DefaultValue
        {
            get { return null; }
        }
        
        protected override object Value
        {
            set { _items = ParseValue(_typeConverter.ConvertToInvariantString(value)); }
            get
            {
                return _typeConverter.ConvertFromInvariantString(
                            ListItems.All(x => x.Checked)
                                ? (string)DefaultValue
                                : ListItems.All(x => !x.Checked)
                                    ? NewItems().ElementAt(0).ToString()
                                    : ListItems.Aggregate(String.Empty, (s, x) => x.Checked ? s + (String.IsNullOrEmpty(s) ? String.Empty : ",") + x.Item : s));
            }
        }

        private readonly CpuAffinityTypeConverter _typeConverter = new CpuAffinityTypeConverter();

        private static IEnumerable<ListItem> ParseValue(string value)
        {
            if (String.IsNullOrEmpty(value) || Validation.IsAutoValue(value))
            {
                return NewItems();
            }

            var items = NewItems(false).ToArray();

            if (String.IsNullOrEmpty(value))
            {
                items[0].Checked = true;
                return items;
            }

            var isOneSet = false;
            foreach (var idStr in value.Split(','))
            {
                int id;
                if (int.TryParse(idStr, out id) && id >= 0 && id < items.Length)
                {
                    items[id].Checked = true;
                    isOneSet = true;
                }
            }

            return !isOneSet ? NewItems() : items;
        }

        private IEnumerable<ListItem> _items;

        protected override IEnumerable<ListItem> ListItems
        {
            set { _items = value; }
            get { return _items; }
        }
    }
}
