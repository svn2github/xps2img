using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using CommandLine.Utils;

using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI.Controls.PropertyGridEx
{
    public partial class PropertyGridEx
    {
        private static readonly Point InvalidPosition  = new Point(int.MinValue, int.MinValue);
        private static readonly Point NegativePosition = new Point(-1, -1);

        private const string ResetItemName    = "PropertyGrid_ResetMenuItem";
        private const string ResetAllItemName = "PropertyGrid_ResetAllMenuItem";

        private const string ResetItemKey     = "PropertyGrid_ResetItem";
        private const string ResetAllItemKey  = "PropertyGrid_ResetAllItem";

        private const string CategorizedKey  = "PropertyGrid_Categorized";
        private const string AlphabeticalKey = "PropertyGrid_Alphabetical";

        private const string ResetTextKeyFormat    = "PropertyGrid_Reset{0}_{1}";
        private const string ResetAllTextKeyFormat = "PropertyGrid_ResetAll{0}";

        private static string ResetItemText
        {
            get { return GetLocalizedString(ResetItemKey, "&Reset {0}"); }
        }

        private static string ResetAllItemText
        {
            get { return GetLocalizedString(ResetAllItemKey, "Reset &All {0}"); }
        }

        private static string GetLocalizedString(string key, string defaultValue)
        {
            if(ResourceManager == null)
            {
                return defaultValue;
            }

            var localizedString = ResourceManager.GetString(key);

            return String.IsNullOrEmpty(localizedString) ? defaultValue : localizedString;
        }

        private void CleanupContextMenuStrip()
        {
            var contextMenuStrip = base.ContextMenuStrip;

            if (contextMenuStrip == null)
            {
                return;
            }

            base.ContextMenuStrip = null;
        }

        private static ToolStripItem CreateContextMenuItem(string name, EventHandler eventHandler)
        {
            var menuItem = new ToolStripMenuItem(name) { Name = name };

            if (eventHandler != null)
            {
                menuItem.Click += eventHandler;
            }

            return menuItem;
        }

        private IEnumerable<GridItem> GetContextMenuGridItems(bool isContextMenuKeyboardOpened)
        {
            if (SelectedGridItem != null && (isContextMenuKeyboardOpened || IsSelectedGridItemUnderCursor()))
            {
                yield return SelectedGridItem;
                yield break;
            }

            foreach(var gridItem in this.FindGridItems(g => g.IsCategory()))
            {
                yield return gridItem;
            }
        }

        private ContextMenuStrip CreateContextMenuStrip(bool isContextMenuKeyboardOpened)
        {
            CleanupContextMenuStrip();

            var contextMenuGridItems = GetContextMenuGridItems(isContextMenuKeyboardOpened).ToArray();
            var contextMenuStrip = new ContextMenuStrip { RenderMode = ContextMenuStripRenderMode };
            
            var menuItems = new List<ToolStripItem>();

            foreach (var gridItem in contextMenuGridItems)
            {
                var gridItemClosure = gridItem;
                var resetMenuItem = CreateContextMenuItem(ResetItemName, (_, __) => ResetMenuItemClickFor(gridItemClosure));

                SetupResetMenuItem(gridItem, resetMenuItem);

                menuItems.Add(resetMenuItem);

                contextMenuStrip.Items.Add(resetMenuItem);
            }

            var resetAllMenuItem = CreateContextMenuItem(ResetAllItemName, (_, __) => ResetAll());
            SetupResetMenuItem(null, resetAllMenuItem);

            var isGridContextMenu = contextMenuGridItems.Length > 1 || !contextMenuGridItems.Contains(SelectedGridItem);
            if (isGridContextMenu)
            {
                if (contextMenuGridItems.Any())
                {
                    contextMenuStrip.Items.Add("-");
                }

                resetAllMenuItem.Enabled = HasSelectedObject && !ReadOnly && IsResetAllEnabled();

                contextMenuStrip.Items.Add(resetAllMenuItem);
            }

            var i = 0;
            foreach (var text in (new HotKeyAssigner()).AssignHotKeysFor(menuItems.Select(m => m.Text), isGridContextMenu ? resetAllMenuItem.Text : String.Empty))
            {
                menuItems[i++].Text = text;
            }

            return contextMenuStrip;
        }

        private void SetupResetMenuItem(GridItem gridItem, ToolStripItem resetMenuItem)
        {
            if (gridItem == null)
            {
                resetMenuItem.Text = String.Format(ResetAllItemText,
                                                   HasSelectedObject
                                                       ? GetLocalizedString(String.Format(ResetAllTextKeyFormat, SelectedObject.GetType().Name), String.Empty)
                                                       : String.Empty);
                return;
            }

            var label = (gridItem.Label ?? String.Empty).Trim();
            var text = label;

            var propertyDescriptor = gridItem.PropertyDescriptor;
            var hasPropertyDescriptor = propertyDescriptor != null;

            if (gridItem.IsCategory() && ResetGroupCallback != null)
            {
                label = this.GetCategoryName(gridItem) ?? label;
            }

            resetMenuItem.Text = String.Format(ResetItemText, GetResetText(hasPropertyDescriptor ? propertyDescriptor.Name : label + "Category", text));

            resetMenuItem.Enabled = !ReadOnly && (
                hasPropertyDescriptor
                    ? HasSelectedObject && propertyDescriptor.CanResetValue(SelectedObject)
                    : ResetGroupCallback != null && ResetGroupCallback(label, true));
        }

        private static readonly string[] CulturesToSkipLowerStripMenuText = { "English" };

        private string GetResetText(string propertyName, string text)
        {
            var str = GetLocalizedString(String.Format(ResetTextKeyFormat, SelectedObject.GetType().Name, propertyName), text);
            return str != text ? str : LowerStripMenuText(str);
        }

        private static string LowerStripMenuText(string text)
        {
            var englishCulltureName = Thread.CurrentThread.CurrentUICulture.EnglishName;

            if (String.IsNullOrEmpty(text) || CulturesToSkipLowerStripMenuText.Any(englishCulltureName.StartsWith) || String.IsNullOrEmpty(englishCulltureName))
            {
                return text;
            }

            var stringBuilder = new StringBuilder(text.Length);

            for (var i = 0; i < text.Length; i++)
            {
                var ch = text[i];
                if (i >= text.Length - 1)
                {
                    stringBuilder.Append(Char.ToLower(ch));
                    break;
                }
                var chNext = text[i+1];
                stringBuilder.Append(Char.IsUpper(ch) && (Char.IsWhiteSpace(chNext) || Char.IsUpper(chNext)) ? ch : Char.ToLower(ch));
            }

            return stringBuilder.ToString();
        }

        private void ResetMenuItemClickFor(GridItem gridItem)
        {
            if (!gridItem.HasPropertyDescriptor())
            {
                if (ResetGroupCallback != null)
                {
                    ResetGroupCallback(gridItem.Label, false);
                }
                return;
            }

            var propertyDescriptor = gridItem.PropertyDescriptor;

            if (propertyDescriptor == null)
            {
                return;
            }

            var oldValue = propertyDescriptor.GetValue(SelectedObject);

            var propertyInfo = propertyDescriptor.ComponentType.GetProperty(propertyDescriptor.Name);

            ReflectionUtils.SetDefaultValue(SelectedObject, propertyInfo);

            Refresh();

            OnPropertyValueChanged(new PropertyValueChangedEventArgs(gridItem, oldValue));
        }

        private bool IsSelectedGridItemUnderCursor(Point? screenPoint = null)
        {
            var point = screenPoint ?? _propertyGridView.PointToClient(Cursor.Position);

            point = (Point)_propertyGridViewFindPositionMethodInfo.Invoke(_propertyGridView, new object[] { point.X, point.Y });
            if (point == InvalidPosition)
            {
                return false;
            }

            var gridEntry = _propertyGridViewGetGridEntryFromRowMethodInfo.Invoke(_propertyGridView, new object[] { point.Y });

            return ReferenceEquals(SelectedGridItem, gridEntry);
        }
    }
}
