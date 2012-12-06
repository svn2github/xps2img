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
    public abstract class CheckedListBoxUITypeEditor<TV> : UITypeEditor
    {
        protected class CheckItem
        {
            public TV Item { get; set; }
            public bool Checked { get; set; }
            public override string ToString()
            {
                return Item.ToString();
            }
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var windowsFormsEditorService = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
            if (windowsFormsEditorService != null)
            {
                Value = value as string;

                var prevValue = Value;

                using (var checkedListBox = new CustomCheckedListBox { CheckOnClick = true, IntegralHeight = true, BorderStyle = BorderStyle.None })
                {
                    foreach (var checkItem in CheckItems)
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

                    CheckItems = checkedListBox.Items.OfType<CheckItem>().ToArray();
                }

                return Value ?? DefaultValue;
            }

            return DefaultValue;
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
            ((CheckItem)((CheckedListBox)sender).Items[itemCheckEventArgs.Index]).Checked = itemCheckEventArgs.NewValue == CheckState.Checked;
        }

        protected abstract object DefaultValue { get; }
        protected abstract object Value { get; set; }

        protected abstract IEnumerable<CheckItem> CheckItems { get; set; }
    }
}
