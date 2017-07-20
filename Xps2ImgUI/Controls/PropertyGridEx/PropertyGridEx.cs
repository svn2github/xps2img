using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

using CommandLine.Utils;

using Xps2ImgUI.Controls.PropertyGridEx.ToolStripEx;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI.Controls.PropertyGridEx
{
    public partial class PropertyGridEx : PropertyGrid, IMessageFilter
    {
        private const bool         UseAutoToolTip    = false;
        private const string       ExtendedCategory  = "Extended";
        private const BindingFlags InstanceNonPublic = BindingFlags.Instance | BindingFlags.NonPublic;

        public PropertyGridEx()
        {
            var controls = Controls.OfType<Control>().ToArray();

            _toolStrip = (ToolStrip)controls.FirstOrDefault(c => c is ToolStrip);
            Debug.Assert(_toolStrip != null);

            var toolStripType = _toolStrip.GetType();

            _updateToolTipMethodInfo = toolStripType.GetMethod("UpdateToolTip", InstanceNonPublic);
            Debug.Assert(_updateToolTipMethodInfo != null);

            _docComment = controls.FirstOrDefault(c => c.GetType().Name == "DocComment");
            Debug.Assert(_docComment != null);

            // ReSharper disable once PossibleNullReferenceException
            var docCommentType = _docComment.GetType();

            _docLinesPropertyInfo = docCommentType.GetProperty("Lines");
            _docFontPropertyInfo  = docCommentType.GetProperty("Font");

            var docUserSizedField = docCommentType.GetField("userSized", InstanceNonPublic);
            if (docUserSizedField != null)
            {
                docUserSizedField.SetValue(_docComment, true);
            }

            _propertyGridView = controls.FirstOrDefault(c => c.GetType().Name == "PropertyGridView");
            Debug.Assert(_propertyGridView != null);

            // ReSharper disable once PossibleNullReferenceException
            var propertyGridViewType = _propertyGridView.GetType();

            Editor = (TextBox)propertyGridViewType.GetProperty("Edit", InstanceNonPublic).GetValue(_propertyGridView, null);
            Debug.Assert(Editor != null);

            _propertyGridViewEnsurePendingChangesCommittedMethodInfo = propertyGridViewType.GetMethod("EnsurePendingChangesCommitted", BindingFlags.Instance | BindingFlags.Public);
            Debug.Assert(_propertyGridViewEnsurePendingChangesCommittedMethodInfo != null);

            _propertyGridViewFindPositionMethodInfo = propertyGridViewType.GetMethod("FindPosition", InstanceNonPublic);
            Debug.Assert(_propertyGridViewFindPositionMethodInfo != null);

            _propertyGridViewGetGridEntryFromRowMethodInfo = propertyGridViewType.GetMethod("GetGridEntryFromRow", InstanceNonPublic);
            Debug.Assert(_propertyGridViewGetGridEntryFromRowMethodInfo != null);

            // Add a custom service provider to give us control over the property value error dialog shown to the user.
            var errorDialogField = propertyGridViewType.GetField("serviceProvider", InstanceNonPublic);
            if (errorDialogField != null)
            {
                errorDialogField.SetValue(_propertyGridView, new PropertyGridExServiceProvider(this));
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            _backColorOriginal = BackColor;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            AddMessageFilter(false);
            CleanupContextMenuStrip();

            base.OnHandleDestroyed(e);
        }

        protected override void OnSelectedObjectsChanged(EventArgs e)
        {
            SetSelectedObjectReadOnly();

            base.OnSelectedObjectsChanged(e);
        }

        protected override void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e)
        {
            UpdateAutoComplete();

            base.OnSelectedGridItemChanged(e);
        }

        private void UpdateAutoComplete()
        {
            Editor.AutoCompleteMode   = AutoCompleteMode.None;
            Editor.AutoCompleteSource = AutoCompleteSource.None;

            if (!AllowAutoComplete || AutoCompleteSettings == null)
            {
                return;
            }

            // ReSharper disable once PossibleNullReferenceException
            var selectedPropName = SelectedGridItem.HasPropertyDescriptor() ? SelectedGridItem.PropertyDescriptor.Name : String.Empty;

            var autoCompleteSettings = AutoCompleteSettings.FirstOrDefault(editAutoComplete => selectedPropName == editAutoComplete.PropName);
            if (autoCompleteSettings == null)
            {
                return;
            }

            Editor.AutoCompleteMode   = autoCompleteSettings.AutoCompleteMode;
            Editor.AutoCompleteSource = autoCompleteSettings.AutoCompleteSource;
        }

        public void RefreshLocalization()
        {
            var firstItemIndex = 0;

            Action<string, string> refreshGridButtons = (k, s) =>
                (_toolStrip.Items.OfType<ToolStripButton>().FirstOrDefault(i => i.Text == s) ?? _toolStrip.Items[firstItemIndex++]).ToolTipText = GetLocalizedString(k, s);

            refreshGridButtons(CategorizedKey,  "Categorized");
            refreshGridButtons(AlphabeticalKey, "Alphabetical");
            
            RefreshLocalization(_toolStrip.Items);
            Refresh();
        }

        private static void RefreshLocalization(IEnumerable items)
        {
            foreach (ToolStripItem item in items)
            {
                var localizableToolStripItem = item as ILocalizableToolStripItem;

                if (localizableToolStripItem != null)
                {
                    localizableToolStripItem.RefreshLocalization();
                }

                var toolStripMenuItem = item as ToolStripDropDownItem;
                if (toolStripMenuItem != null)
                {
                    RefreshLocalization(toolStripMenuItem.DropDownItems);
                }
            }
        }

        public void RemoveLastToolStripItem()
        {
            var lastIndex = _toolStrip.Items.Count - 1;
            if (lastIndex >= 0)
            {
                _toolStrip.Items.RemoveAt(lastIndex);
            }
        }

        public ToolStripSplitButton AddToolStripSplitButton(Func<string> updateText, Func<string> updateToolTipText, Func<Image> updateImage, EventHandler eventHandler, params ToolStripButtonItem[] items)
        {
            return AddToolStripSplitButton(updateImage != null ? updateImage() : null, updateText, updateToolTipText, updateImage, eventHandler, items);
        }

        public ToolStripSplitButton AddToolStripSplitButton(Image image, Func<string> updateText, Func<string> updateToolTipText, Func<Image> updateImage, EventHandler eventHandler, params ToolStripButtonItem[] items)
        {
            var toolStripSplitButton = new ToolStripSplitButtonEx(updateText, updateToolTipText, updateImage)
            {
                Image = image,
                AutoToolTip = UseAutoToolTip,
                ImageScaling = ToolStripItemImageScaling.None
            };
            return AddToolStripSplitButton(toolStripSplitButton, eventHandler, items);
        }

        public ToolStripSplitButton AddToolStripSplitButton(Image image, Func<string> updateToolTipText, EventHandler eventHandler, params ToolStripButtonItem[] items)
        {
            return AddToolStripSplitButton(image, null, updateToolTipText, null, eventHandler, items);
        }

        public ToolStripSplitButton AddToolStripSplitButton(ToolStripSplitButton toolStripSplitButton, EventHandler eventHandler, params ToolStripButtonItem[] items)
        {
            toolStripSplitButton.DropDownOpening += (sender, eventArgs) => UpdateToolTip(null);

            _toolStrip.Items.Add(toolStripSplitButton);
            toolStripSplitButton.ButtonClick += eventHandler;

            foreach (var item in items)
            {
                if (item.IsSeparator)
                {
                    toolStripSplitButton.DropDownItems.Add(new ToolStripSeparator());
                    continue;
                }

                var itemControl = new ToolStripMenuItemEx(item.TextFunc) { AutoToolTip = UseAutoToolTip };
                itemControl.Click += item.EventHandler;

                toolStripSplitButton.DropDownItems.Add(itemControl);

                item.ToolStripItem = itemControl;
            }

            return toolStripSplitButton;
        }

        public ToolStripButton AddToolStripButton(Image image, Func<string> toolTipText, EventHandler eventHandler)
        {
            var toolStripButton = new ToolStripButtonEx(toolTipText) { Image = image, AutoToolTip = UseAutoToolTip, ImageScaling = ToolStripItemImageScaling.None };
            return AddToolStripButton(toolStripButton, eventHandler);
        }

        private ToolStripButton AddToolStripButton(ToolStripButton toolStripButton, EventHandler eventHandler)
        {
            toolStripButton.Click += eventHandler;
            _toolStrip.Items.Add(toolStripButton);
            return toolStripButton;
        }

        public void AddToolStripSeparator(bool alignRight = false)
        {
            _toolStrip.Items.Add(new ToolStripSeparator { Alignment = alignRight ? ToolStripItemAlignment.Right : ToolStripItemAlignment.Left });
        }

        private void MoveSplitterTo(int x)
        {
            _propertyGridView.GetType()
                .GetMethod("MoveSplitterTo", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(_propertyGridView, new object[] { x });
        }

        public void MoveSplitterByPercent(int percent)
        {
            MoveSplitterTo(_propertyGridView.Width * percent / 100);
        }

        public void SetDocMonospaceFont()
        {
            var fonts = new[]
            {
                new { Name = "Consolas",                Scale = 1.005 },
                new { Name = "Lucida Sans Typewriter",  Scale = 1.0   },
                new { Name = "Courier New",             Scale = 1.0   },
                new { Name = "Lucida Console",          Scale = 1.001 }
            };

            foreach (var font in fonts)
            {
                var newFont = new Font(font.Name, (float)(DocFont.Size * font.Scale), DocFont.Style, DocFont.Unit);
                if (newFont.Name != "Microsoft Sans Serif")
                {
                    DocFont = newFont;
                    break;
                }
                newFont.Dispose();
            }
        }

        public void EnsurePendingChangesCommitted()
        {
            _propertyGridViewEnsurePendingChangesCommittedMethodInfo.Invoke(_propertyGridView, new object[0]);
        }

        public new bool Validate()
        {
            HasErrors = false;
            _propertyGridViewEnsurePendingChangesCommittedMethodInfo.Invoke(_propertyGridView, new object[0]);
            var isValid = !HasErrors;
            HasErrors = false;
            return isValid;
        }

        private readonly PropertyInfo _docLinesPropertyInfo;

        public void UpdateLayout()
        {
            OnResize(EventArgs.Empty);
        }

        public new void Focus()
        {
            _propertyGridView.Focus();
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Focused
        {
            get { return base.Focused || _propertyGridView.Focused || Editor.Focused; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int DocLines
        {
            get { return (int)_docLinesPropertyInfo.GetValue(_docComment, null); }
            set { _docLinesPropertyInfo.SetValue(_docComment, value, null); }
        }

        private readonly PropertyInfo _docFontPropertyInfo;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Font DocFont
        {
            get { return (Font)_docFontPropertyInfo.GetValue(_docComment, null); }
            set { _docFontPropertyInfo.SetValue(_docComment, value, null); }
        }

        private bool _readOnly;
        
        [Category(ExtendedCategory)]
        [DefaultValue(false)]
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                SetSelectedObjectReadOnly();
            }
        }

        [Category(ExtendedCategory)]
        [DefaultValue(false)]
        public bool ModernLook
        {
            get { return DrawFlatToolbar; }
            set
            {
                DrawFlatToolbar = value;
                BackColor = _backColorOriginal;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ToolStripRenderMode ContextMenuStripRenderMode
        {
            get { return ModernLook ? ToolStripRenderMode.ManagerRenderMode : ToolStripRenderMode.System; }
        }

        private void SetSelectedObjectReadOnly()
        {
            if (!HasSelectedObject)
            {
                return;
            }

            TypeDescriptor.AddAttributes(SelectedObject, new ReadOnlyAttribute(_readOnly));
            Refresh();
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get { return base.ContextMenuStrip; }
            set { throw new InvalidOperationException("Internal use"); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TextBox Editor
        {
            get; private set;
        }

        // string label - property label
        // bool check   - check requested if true; otherwise reset should be executed
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<string, bool, bool> ResetGroupCallback { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Action ResetAllAction { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<PropertyInfo, bool> ResetByCategoryFilter { get; set; }

        public bool IsResetAllEnabled()
        {
            return IsResetByCategoryEnabled(null);
        }

        public bool IsResetByCategoryEnabled(string category)
        {
            var useCategory = !String.IsNullOrEmpty(category);

            return !ReflectionUtils.ForEachPropertyInfo(SelectedObject, pi =>
            {
                var categoryAttribute = pi.FirstOrDefaultAttribute<CategoryAttribute>();

                if (categoryAttribute == null || (useCategory && category != categoryAttribute.Category) || (ResetByCategoryFilter != null && !ResetByCategoryFilter(pi)))
                {
                    return true;
                }

                var defaultValue = pi.FirstOrNewAttribute(() => new DefaultValueAttribute(null)).Value;
                var propertyValue = pi.GetValue(SelectedObject, null);

                return defaultValue == null ? propertyValue == null : defaultValue.Equals(propertyValue);
            });
        }

        public void ResetAll()
        {
            if (ResetAllAction != null)
            {
                ResetAllAction();
                return;
            }

            ResetByCategory(null);
        }

        public void ResetByCategory(string category, bool isLabel = true)
        {
            var categoryName = category;

            var useCategory = !String.IsNullOrEmpty(category);

            if (useCategory && isLabel)
            {
                var gridItem = this.FindGridItem(g => g.Label == category && g.IsCategory());
                if (gridItem == null)
                {
                    return;
                }

                categoryName = this.GetCategoryName(gridItem);
            }

            ReflectionUtils.SetDefaultValues(SelectedObject, pi =>
                (!useCategory || categoryName == pi.FirstOrNewAttribute<CategoryAttribute>().Category) &&
                (ResetByCategoryFilter == null || ResetByCategoryFilter(pi))
            );

            Refresh();
        }

        private void UpdateToolTip(object obj)
        {
            _updateToolTipMethodInfo.Invoke(_toolStrip, new[] { obj });
        }

        public void UpdateToolStripToolTip()
        {
            var oldValue = CurrentlyActiveTooltipItem.GetValue(_toolStrip);

            CurrentlyActiveTooltipItem.SetValue(_toolStrip, null);

            UpdateToolTip(oldValue);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ToolStripRenderer ToolStripRenderer
        {
            get { return base.ToolStripRenderer; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasSelectedObject
        {
            get { return SelectedObject != null; }
        }
        
        public new object SelectedObject
        {
            get { return base.SelectedObject; }
            set
            {
                HasErrors = false;
                base.SelectedObject = value;
            }
        }

        public new object[] SelectedObjects
        {
            get { return base.SelectedObjects; }
            set
            {
                HasErrors = false;
                base.SelectedObjects = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasErrors { get; internal set; }

        private bool _allowAutoComplete;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowAutoComplete
        {
            get { return _allowAutoComplete; }
            set
            {
                _allowAutoComplete = value;
                UpdateAutoComplete();
            }
        }

        private IEnumerable<EditAutoComplete> _autoCompleteSettings;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<EditAutoComplete> AutoCompleteSettings
        {
            get { return _autoCompleteSettings; }
            set
            {
                _autoCompleteSettings = value;
                UpdateAutoComplete();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<string> UseF4OnDoubleClickForProperties
        {
            get { return _useF4OnDoubleClickForProperties; }
            set
            {
                if (_useF4OnDoubleClickForProperties == null)
                {
                    _useF4OnDoubleClickForProperties = new HashSet<string>();
                }

                foreach (var propertyName in value)
                {
                    _useF4OnDoubleClickForProperties.Add(propertyName);
                }

                AddMessageFilter(true);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_CONTEXTMENU)
            {
                base.ContextMenuStrip = CreateContextMenuStrip(GetPoint(m.LParam) == NegativePosition);
            }

            base.WndProc(ref m);
        }

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            if (_readOnly && m.Msg == WM_KEYDOWN && m.WParam.ToInt32() == VK_DELETE && Focused)
            {
                return true;
            }

            if (m.Msg != WM_LBUTTONDBLCLK || _propertyGridView.Handle != m.HWnd || !IsSelectedGridItemUnderCursor(GetPoint(m.LParam)))
            {
                return false;
            }
            
            var propertyDescriptor = SelectedGridItem.PropertyDescriptor;
            if (propertyDescriptor != null && _useF4OnDoubleClickForProperties != null && _useF4OnDoubleClickForProperties.Contains(propertyDescriptor.Name))
            {
                SendKeys.Send("{F4}");
                return true;
            }

            return false;
        }

        private void AddMessageFilter(bool add)
        {
            if (_isMessageFilterAdded && add)
            {
                return;
            }

            if (!_isMessageFilterAdded && !add)
            {
                return;
            }

            if (add)
            {
                Application.AddMessageFilter(this);
            }
            else
            {
                Application.RemoveMessageFilter(this);
            }

            _isMessageFilterAdded = add;
        }

        [Browsable(false)]
        public static ResourceManager ResourceManager { get; set; }

        private readonly ToolStrip _toolStrip;
        private readonly Control _docComment;

        private readonly Control _propertyGridView;

        private readonly MethodInfo _propertyGridViewEnsurePendingChangesCommittedMethodInfo;
        private readonly MethodInfo _propertyGridViewFindPositionMethodInfo;
        private readonly MethodInfo _propertyGridViewGetGridEntryFromRowMethodInfo;

        private readonly MethodInfo _updateToolTipMethodInfo;

        private Color _backColorOriginal;

        private bool _isMessageFilterAdded;
        private HashSet<string> _useF4OnDoubleClickForProperties;

        private FieldInfo CurrentlyActiveTooltipItem
        {
            get { return _toolStrip.GetType().GetField("currentlyActiveTooltipItem", InstanceNonPublic); }
        }
    }
}
