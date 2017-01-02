using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Xps2Img.Shared.Controls;

namespace Xps2Img.Shared.TypeEditors
{
    public abstract class CheckedListBoxEditor<T> : UITypeEditor
    {
        protected class ListItem
        {
            public T Item { get; set; }
            public bool Checked { get; set; }
            public override string ToString()
            {
                return Item.ToString();
            }
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider == null)
            {
                return DefaultValue;
            }

            var windowsFormsEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (windowsFormsEditorService == null)
            {
                return DefaultValue;
            }

            var prevValue = Value = value;

            using (var checkedListBox = new CustomCheckedListBox { CheckOnClick = true, IntegralHeight = true, BorderStyle = BorderStyle.None })
            {
                foreach (var checkItem in ListItems)
                {
                    checkedListBox.Items.Add(checkItem, checkItem.Checked);
                }

                checkedListBox.ItemCheck += CheckedListBoxOnItemCheck;
                windowsFormsEditorService.DropDownControl(checkedListBox);
                checkedListBox.ItemCheck -= CheckedListBoxOnItemCheck;

                if (checkedListBox.ExitedByEscape)
                {
                    return prevValue ?? DefaultValue;
                }

                ListItems = checkedListBox.Items.OfType<ListItem>().ToArray();
            }

            return Value ?? DefaultValue;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override bool IsDropDownResizable
        {
            get { return true; }
        }

        private static void CheckedListBoxOnItemCheck(object sender, ItemCheckEventArgs itemCheckEventArgs)
        {
            ((ListItem)((CheckedListBox)sender).Items[itemCheckEventArgs.Index]).Checked = itemCheckEventArgs.NewValue == CheckState.Checked;
        }

        protected abstract object DefaultValue { get; }
        protected abstract object Value { get; set; }

        protected abstract IEnumerable<ListItem> ListItems { get; set; }
    }
}
