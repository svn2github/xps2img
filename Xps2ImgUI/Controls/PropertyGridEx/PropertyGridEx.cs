using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls.PropertyGridEx
{
    public class PropertyGridEx : PropertyGrid
    {
        private const bool UseAutoToolTip = false;

        private const string ExtendedCategory = "Extended";

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            _backColorOriginal = BackColor;

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

            foreach (Control control in Controls)
            {
                if (String.Compare(control.GetType().FullName, "System.Windows.Forms.PropertyGridInternal.PropertyGridView", true, CultureInfo.InvariantCulture) != 0)
                {
                    continue;
                }

                // Add a custom service provider to give us control over the property value error dialog shown to the user.
                var errorDialogField = control.GetType().GetField("serviceProvider", BindingFlags.Instance | BindingFlags.NonPublic);
                if (errorDialogField != null)
                {
                    errorDialogField.SetValue(control, new PropertyGridExServiceProvider(this));
                }
            }
        }

        protected override void OnSelectedObjectsChanged(EventArgs e)
        {
            SetSelectedObjectReadOnly();
            base.OnSelectedObjectsChanged(e);
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

        public void SetDocMonospaceFont()
        {
            var fonts = new[]
                        {
                            new { Name = "Consolas", Scale = 1.006 },
                            new { Name = "Lucida Sans Typewriter", Scale = 0.995 },
                            new { Name = "Courier New", Scale = 1.005 },
                            new { Name = "Lucida Console", Scale = 1.001 }
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

        private PropertyInfo _docLinesPropertyInfo;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int DocLines
        {
            get { return (int)_docLinesPropertyInfo.GetValue(_docComment, null); }
            set { _docLinesPropertyInfo.SetValue(_docComment, value, null); }
        }

        private PropertyInfo _docFontPropertyInfo;

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
                TypeDescriptor.AddAttributes(SelectedObject, new [] { new ReadOnlyAttribute(_readOnly) });
                Refresh();
            }
        }

        private ToolStrip _toolStrip;
        private Control _docComment;

        private Type _docCommentType;

        private Color _backColorOriginal;
    }
}
