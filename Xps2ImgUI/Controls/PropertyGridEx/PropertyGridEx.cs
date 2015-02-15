using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using CommandLine.Utils;

using Xps2ImgUI.Controls.PropertyGridEx.ToolStripEx;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI.Controls.PropertyGridEx
{
    public class PropertyGridEx : PropertyGrid
    {
        public class EditAutoComplete
        {
            public EditAutoComplete(string propName, AutoCompleteSource autoCompleteSource, AutoCompleteMode autoCompleteMode = AutoCompleteMode.SuggestAppend)
            {
                PropName = propName;
                AutoCompleteMode = autoCompleteMode;
                AutoCompleteSource = autoCompleteSource;
            }

            public readonly string PropName;
            public readonly AutoCompleteMode AutoCompleteMode;
            public readonly AutoCompleteSource AutoCompleteSource;
        }

        private const bool UseAutoToolTip = false;

        private const string ExtendedCategory = "Extended";

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
            _docFontPropertyInfo = docCommentType.GetProperty("Font");

            var docUserSizedField = docCommentType.GetField("userSized", InstanceNonPublic);
            if (docUserSizedField != null)
            {
                docUserSizedField.SetValue(_docComment, true);
            }

            _propertyGridView = controls.FirstOrDefault(c => c.GetType().Name == "PropertyGridView");
            Debug.Assert(_propertyGridView != null);

            // ReSharper disable once PossibleNullReferenceException
            var propertyGridViewType = _propertyGridView.GetType();

            _propertyGridViewEdit = (TextBox)propertyGridViewType.GetProperty("Edit", InstanceNonPublic).GetValue(_propertyGridView, null);
            Debug.Assert(_propertyGridViewEdit != null);

            _propertyGridViewEnsurePendingChangesCommitted = propertyGridViewType.GetMethod("EnsurePendingChangesCommitted", BindingFlags.Instance | BindingFlags.Public);
            Debug.Assert(_propertyGridViewEnsurePendingChangesCommitted != null);

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
            _propertyGridViewEdit.AutoCompleteMode = AutoCompleteMode.None;
            _propertyGridViewEdit.AutoCompleteSource = AutoCompleteSource.None;

            if (!AllowAutoComplete || AutoCompleteSettings == null)
            {
                return;
            }

            var selectedPropName = SelectedGridItem.HasPropertyDescriptor() ? SelectedGridItem.PropertyDescriptor.Name : String.Empty;

            var autoCompleteSettings = AutoCompleteSettings.FirstOrDefault(editAutoComplete => selectedPropName == editAutoComplete.PropName);
            if (autoCompleteSettings == null)
            {
                return;
            }

            _propertyGridViewEdit.AutoCompleteMode = autoCompleteSettings.AutoCompleteMode;
            _propertyGridViewEdit.AutoCompleteSource = autoCompleteSettings.AutoCompleteSource;
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

        public ToolStripSplitButton AddToolStripSplitButton(Image image, Func<string> toolTipText, EventHandler eventHandler, params ToolStripButtonItem[] items)
        {
            var toolStripSplitButton = new ToolStripSplitButtonEx(toolTipText) { Image = image, AutoToolTip = UseAutoToolTip, ImageScaling = ToolStripItemImageScaling.None };
            return AddToolStripSplitButton(toolStripSplitButton, eventHandler, items);
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

        public void AddToolStripSeparator()
        {
            _toolStrip.Items.Add(new ToolStripSeparator());
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
            _propertyGridViewEnsurePendingChangesCommitted.Invoke(_propertyGridView, new object[0]);
        }

        public new bool Validate()
        {
            HasErrors = false;
            _propertyGridViewEnsurePendingChangesCommitted.Invoke(_propertyGridView, new object[0]);
            var isValid = !HasErrors;
            HasErrors = false;
            return isValid;
        }

        private readonly PropertyInfo _docLinesPropertyInfo;

        public void UpdateLayout()
        {
            OnResize(EventArgs.Empty);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Focused
        {
            get { return base.Focused || _propertyGridView.Focused || _propertyGridViewEdit.Focused; }
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
                ContextMenuStrip.RenderMode = value ? ToolStripRenderMode.ManagerRenderMode : ToolStripRenderMode.System;
            }
        }

        private void SetSelectedObjectReadOnly()
        {
            if (SelectedObject != null)
            {
                TypeDescriptor.AddAttributes(SelectedObject, new ReadOnlyAttribute(_readOnly));
                Refresh();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                InitContextMenuStrip();
                return base.ContextMenuStrip;
            }
            set { throw new InvalidOperationException("Control uses its on menu."); }
        }

        // string label - property label
        // bool check   - check requested if true; otherwise reset should be executed
        public Func<string, bool, bool> ResetGroupCallback { get; set; }

        private const string ResetItemName = "reset";
        private const string CloseItemName = "close";

        private const string ResetItemKey = "PropertyGrid_ResetItem";
        private const string CloseItemKey = "PropertyGrid_CloseItem";

        private const string CategorizedKey  = "PropertyGrid_Categorized";
        private const string AlphabeticalKey = "PropertyGrid_Alphabetical";

        private const string ResetTextKeyFormat = "PropertyGrid_Reset{0}";

        private static string ResetItemText
        {
            get { return GetLocalizedString(ResetItemKey, "&Reset {0}"); }
        }

        private static string CloseItemText
        {
            get { return GetLocalizedString(CloseItemKey, "&Close"); }
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

        private void InitContextMenuStrip()
        {
            if (base.ContextMenuStrip != null)
            {
                return;
            }

            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Opening += ContextStripMenuOpening;

            var resetMenuItem = contextMenuStrip.Items.Add(ResetItemText);
            resetMenuItem.Name = ResetItemName;
            resetMenuItem.Click += ResetMenuItemClick;

            contextMenuStrip.Items.Add("-");

            var closeItemText = contextMenuStrip.Items.Add(CloseItemText);
            closeItemText.Name = CloseItemName;

            base.ContextMenuStrip = contextMenuStrip;
        }

        private static readonly string[] CulturesToSkipLowerStripMenuText = { "English" };

        private static string GetResetText(string propertyName, string text)
        {
            var str = GetLocalizedString(String.Format(ResetTextKeyFormat, propertyName), text);
            return str != text ? str : LowerStripMenuText(str);
        }

        private static string LowerStripMenuText(string text)
        {
            var englishCulltureName = Thread.CurrentThread.CurrentUICulture.EnglishName;

            if (String.IsNullOrEmpty(text) || CulturesToSkipLowerStripMenuText.Any(n => englishCulltureName.StartsWith(n)) || String.IsNullOrEmpty(englishCulltureName))
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

        private void ContextStripMenuOpening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip.Items[CloseItemName].Text = CloseItemText;

            var resetMenuItem = ContextMenuStrip.Items[ResetItemName];
            var label = (SelectedGridItem.Label ?? String.Empty).Trim();
            var text = label;

            var propertyDescriptor = SelectedGridItem.PropertyDescriptor;
            var hasPropertyDescriptor = propertyDescriptor != null;

            if(SelectedGridItem.IsCategory() && ResetGroupCallback != null)
            {
                label = this.GetCategoryName() ?? label;
            }

            resetMenuItem.Text = String.Format(ResetItemText, GetResetText(hasPropertyDescriptor ? propertyDescriptor.Name : label, text));

            resetMenuItem.Enabled = !ReadOnly &&
                                    (
                                        (!hasPropertyDescriptor && ResetGroupCallback != null && ResetGroupCallback(label, true)) ||
                                        (hasPropertyDescriptor && propertyDescriptor.CanResetValue(SelectedObject))
                                    );
        }

        private void ResetMenuItemClick(object sender, EventArgs e)
        {
            if (!SelectedGridItem.HasPropertyDescriptor())
            {
                if (ResetGroupCallback != null)
                {
                    ResetGroupCallback(SelectedGridItem.Label, false);
                }
                return;
            }

            var propertyDescriptor = SelectedGridItem.PropertyDescriptor;

            if (propertyDescriptor == null)
            {
                return;
            }

            var oldValue = propertyDescriptor.GetValue(SelectedObject);

            var propertyInfo = propertyDescriptor.ComponentType.GetProperty(propertyDescriptor.Name);

            ReflectionUtils.SetDefaultValue(SelectedObject, propertyInfo);

            Refresh();

            OnPropertyValueChanged(new PropertyValueChangedEventArgs(SelectedGridItem, oldValue));
        }

        public bool IsResetByCategoryEnabled(string category, Func<PropertyInfo, bool> allowFilter = null)
        {
            return !ReflectionUtils.ForEachPropertyInfo(SelectedObject, pi =>
            {
                if(category != pi.FirstOrNewAttribute<CategoryAttribute>().Category || (allowFilter != null && !allowFilter(pi)))
                {
                    return true;
                }

                var defaultValue = pi.FirstOrNewAttribute(() => new DefaultValueAttribute(null)).Value;
                var propertyValue = pi.GetValue(SelectedObject, null);

                return defaultValue == null ? propertyValue == null : defaultValue.Equals(propertyValue);
            });
        }

        public void ResetByCategory(string category, Func<PropertyInfo, bool> allowFilter = null)
        {
            // ReSharper disable once AccessToModifiedClosure
            category = this.GetCategoryName(this.FindGridItem(g => g.Label == category && g.IsCategory())) ?? category;

            ReflectionUtils.SetDefaultValues(SelectedObject, pi =>  
                category == pi.FirstOrNewAttribute<CategoryAttribute>().Category &&
                (allowFilter == null || allowFilter(pi))
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
        public static ResourceManager ResourceManager { get; set; }

        private readonly ToolStrip _toolStrip;
        private readonly Control _docComment;

        private readonly Control _propertyGridView;
        private readonly TextBox _propertyGridViewEdit;
        private readonly MethodInfo _propertyGridViewEnsurePendingChangesCommitted;

        private readonly MethodInfo _updateToolTipMethodInfo;

        private Color _backColorOriginal;

        private FieldInfo CurrentlyActiveTooltipItem
        {
            get { return _toolStrip.GetType().GetField("currentlyActiveTooltipItem", InstanceNonPublic); }
        }
    }
}
