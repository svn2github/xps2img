using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls.PropertyGridEx
{
    public class PropertyGridEx : PropertyGrid
    {
        public class EditAutoComplete
        {
            public EditAutoComplete(string label, AutoCompleteSource autoCompleteSource, AutoCompleteMode autoCompleteMode = AutoCompleteMode.SuggestAppend)
            {
                Label = label;
                AutoCompleteMode = autoCompleteMode;
                AutoCompleteSource = autoCompleteSource;
            }

            public readonly string Label;
            public readonly AutoCompleteMode AutoCompleteMode;
            public readonly AutoCompleteSource AutoCompleteSource;
        }

        private const bool UseAutoToolTip = false;

        private const string ExtendedCategory = "Extended";

        public PropertyGridEx()
        {
            var controls = Controls.OfType<Control>().ToArray();

            _toolStrip = (ToolStrip)controls.FirstOrDefault(c => c is ToolStrip);
            Debug.Assert(_toolStrip != null);

            _docComment = controls.FirstOrDefault(c => c.GetType().Name == "DocComment");
            Debug.Assert(_docComment != null);

            _docCommentType = _docComment.GetType();

            _docLinesPropertyInfo = _docCommentType.GetProperty("Lines");
            _docFontPropertyInfo = _docCommentType.GetProperty("Font");

            var docUserSizedField = _docCommentType.GetField("userSized", BindingFlags.Instance | BindingFlags.NonPublic);
            if (docUserSizedField != null)
            {
                docUserSizedField.SetValue(_docComment, true);
            }

            _propertyGridView = controls.FirstOrDefault(c => c.GetType().Name == "PropertyGridView");
            Debug.Assert(_propertyGridView != null);

            var propertyGridViewType = _propertyGridView.GetType();

            _propertyGridViewEdit = (TextBox)propertyGridViewType.GetProperty("Edit", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(_propertyGridView, null);
            Debug.Assert(_propertyGridViewEdit != null);

            // Add a custom service provider to give us control over the property value error dialog shown to the user.
            var errorDialogField = propertyGridViewType.GetField("serviceProvider", BindingFlags.Instance | BindingFlags.NonPublic);
            if (errorDialogField != null)
            {
                errorDialogField.SetValue(_propertyGridView, new PropertyGridExServiceProvider(this));
            }

            InitContextMenuStrip();
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

        private string _selectedItemLabel;

        protected override void OnSelectedGridItemChanged(SelectedGridItemChangedEventArgs e)
        {
            _selectedItemLabel = e.NewSelection.Label;

            UpdateAutoComplete();

            base.OnSelectedGridItemChanged(e);
        }

        private void UpdateAutoComplete()
        {
            _propertyGridViewEdit.AutoCompleteMode = AutoCompleteMode.None;
            _propertyGridViewEdit.AutoCompleteSource = AutoCompleteSource.None;

            if (AllowAutoComplete && AutoCompleteSettings != null)
            {
                var autoCompleteSettings = AutoCompleteSettings.FirstOrDefault(editAutoComplete => _selectedItemLabel == editAutoComplete.Label);
                if (autoCompleteSettings != null)
                {
                    _propertyGridViewEdit.AutoCompleteMode = autoCompleteSettings.AutoCompleteMode;
                    _propertyGridViewEdit.AutoCompleteSource = autoCompleteSettings.AutoCompleteSource;
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

        public ToolStripSplitButton AddToolStripSplitButton(string text, EventHandler eventHandler, params ToolStripButtonItem[] items)
        {
            var toolStripSplitButton = new ToolStripSplitButton(text) { AutoToolTip = UseAutoToolTip };

            _toolStrip.Items.Add(toolStripSplitButton);
            toolStripSplitButton.ButtonClick += eventHandler;

            foreach (var item in items)
            {
                if (item.IsSeparator)
                {
                    toolStripSplitButton.DropDownItems.Add(new ToolStripSeparator());
                }
                else
                {
                    var itemControl = toolStripSplitButton.DropDownItems.Add(item.Text);
                    itemControl.AutoToolTip = UseAutoToolTip;
                    itemControl.Click += item.EventHandler;

                    item.ToolStripItem = itemControl;
                }
            }

            return toolStripSplitButton;
        }

        public ToolStripButton AddToolStripButton(string text, EventHandler eventHandler)
        {
            var toolStripButton = new ToolStripButton(text) { AutoToolTip = UseAutoToolTip };
            toolStripButton.Click += eventHandler;
            _toolStrip.Items.Add(toolStripButton);
            return toolStripButton;
        }

        public void AddToolStripSeparator()
        {
            _toolStrip.Items.Add(new ToolStripSeparator());
        }

        protected void MoveSplitterTo(int x)
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
                new { Name = "Lucida Sans Typewriter",  Scale = 1.0 },
                new { Name = "Courier New",             Scale = 1.0 },
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

        private readonly PropertyInfo _docLinesPropertyInfo;

        public void UpdateLayout()
        {
            OnResize(EventArgs.Empty);
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

        private void SetSelectedObjectReadOnly()
        {
            if (SelectedObject != null)
            {
                TypeDescriptor.AddAttributes(SelectedObject, new Attribute[] { new ReadOnlyAttribute(_readOnly) });
                Refresh();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get { return base.ContextMenuStrip; }
            set { throw new InvalidOperationException("Control uses its on menu."); }
        }

        // string label - property label
        // bool check   - check requested if true; otherwise reset should be executed
        public Func<string, bool, bool> ResetGroupCallback { get; set; }

        private const string ResetItemText = "&Reset {0}";
        private const string CloseItemText = "&Close";
        private const string ResetItemName = "reset";

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
            contextMenuStrip.Items.Add(CloseItemText);

            base.ContextMenuStrip = contextMenuStrip;
        }

        private void ContextStripMenuOpening(object sender, CancelEventArgs e)
        {
            var resetMenuItem = ContextMenuStrip.Items[ResetItemName];
            var label = SelectedGridItem.Label;

            var hasPropertyDescriptor = SelectedGridItem.PropertyDescriptor != null;

            resetMenuItem.Text = String.Format(ResetItemText, label);
            resetMenuItem.Enabled = !ReadOnly &&
                                    (
                                        (!hasPropertyDescriptor && ResetGroupCallback != null && ResetGroupCallback(label, true)) ||
                                        (hasPropertyDescriptor && SelectedGridItem.PropertyDescriptor.CanResetValue(SelectedObject))
                                    );
        }

        private void ResetMenuItemClick(object sender, EventArgs e)
        {
            if (SelectedGridItem.PropertyDescriptor == null)
            {
                if (ResetGroupCallback != null)
                {
                    ResetGroupCallback(SelectedGridItem.Label, false);
                }
                return;
            }

            var oldValue = SelectedGridItem.PropertyDescriptor.GetValue(SelectedObject);

            ResetSelectedProperty();

            OnPropertyValueChanged(new PropertyValueChangedEventArgs(SelectedGridItem, oldValue));
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
        public bool HasErrors { get; set; }

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

        private readonly ToolStrip _toolStrip;
        private readonly Control _docComment;

        private readonly Control _propertyGridView;
        private readonly TextBox _propertyGridViewEdit;

        private readonly Type _docCommentType;

        private Color _backColorOriginal;
    }
}
