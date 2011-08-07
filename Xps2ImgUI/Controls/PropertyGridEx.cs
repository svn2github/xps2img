using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls
{
    public class PropertyGridEx : PropertyGrid
    {
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            _toolStrip = Controls.OfType<ToolStrip>().FirstOrDefault();
            Debug.Assert(_toolStrip != null);
        
            _docComment = Controls.OfType<Control>().Where(c => c.GetType().Name == "DocComment").FirstOrDefault();
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

        public void RemoveLastToolStripButton()
        {
            if (_toolStrip.Items.Count > 0)
            {
                _toolStrip.Items.RemoveAt(_toolStrip.Items.Count - 1);
            }
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

        private ToolStrip _toolStrip;
        private Control _docComment;
        private Type _docCommentType;
    }
}
