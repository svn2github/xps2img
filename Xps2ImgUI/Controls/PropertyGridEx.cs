using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls
{
    public class ToolStripButtonItem
    {
        public readonly string Text;
        public readonly EventHandler EventHandler;

        public bool IsSeparator { get { return Text == null; } }

        public ToolStripButtonItem()
        {
        }

        public ToolStripButtonItem(string text, EventHandler eventHandler)
        {
            Text = text;
            EventHandler = eventHandler;
        }
    }

    public class PropertyGridEx : PropertyGrid
    {
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            var controls = Controls.OfType<Control>().ToArray();

            _toolStrip = (ToolStrip)controls.Where(c => c is ToolStrip).FirstOrDefault();
            Debug.Assert(_toolStrip != null);

            _docComment = controls.Where(c => c.GetType().Name == "DocComment").FirstOrDefault();
            Debug.Assert(_docComment != null);

            _docCommentType = _docComment.GetType();

            _docLinesPropertyInfo = _docCommentType.GetProperty("Lines");
            _docFontPropertyInfo = _docCommentType.GetProperty("Font");

            var docUserSizedField = _docCommentType.GetField("userSized", BindingFlags.Instance | BindingFlags.NonPublic);
            if (docUserSizedField != null)
            {
                docUserSizedField.SetValue(_docComment, true);
            }
        }

        protected override void OnSelectedObjectsChanged(EventArgs e)
        {
            SetSelectedObjectReadOnly();
            base.OnSelectedObjectsChanged(e);
        }

        public void RemoveLastToolStripButton()
        {
            if (_toolStrip.Items.Count > 0)
            {
                _toolStrip.Items.RemoveAt(_toolStrip.Items.Count - 1);
            }
        }

        public ToolStripSplitButton AddToolStripSplitButton(string text, EventHandler eventHandler, params ToolStripButtonItem[] items)
        {
            var toolStripSplitButton = new ToolStripSplitButton(text);

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
                    itemControl.Click += item.EventHandler;
                }
            }

            return toolStripSplitButton;
        }

        public ToolStripButton AddToolStripButton(string text, EventHandler eventHandler)
        {
            var toolStripButton = new ToolStripButton(text);
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
        public int DocLines
        {
            get { return (int)_docLinesPropertyInfo.GetValue(_docComment, null); }
            set { _docLinesPropertyInfo.SetValue(_docComment, value, null); }
        }

        private PropertyInfo _docFontPropertyInfo;
        public Font DocFont
        {
            get { return (Font)_docFontPropertyInfo.GetValue(_docComment, null); }
            set { _docFontPropertyInfo.SetValue(_docComment, value, null); }
        }

        private bool _readOnly;
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                SetSelectedObjectReadOnly();
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
    }
}
