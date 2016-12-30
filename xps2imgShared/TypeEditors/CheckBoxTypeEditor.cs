using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;

namespace Xps2Img.Shared.TypeEditors
{
    public class CheckBoxTypeEditor : UITypeEditor
    {
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            var bounds   = e.Bounds;
            var context  = e.Context;
            var graphics = e.Graphics;
            
            var stateImage = (bool)e.Value ? Resources.Images.Checked : Resources.Images.Unchecked;

            if (IsValueEditable(context))
            {
                graphics.DrawImage(stateImage, bounds);
            }
            else
            {
                ControlPaint.DrawImageDisabled(graphics, stateImage, bounds.X, bounds.Y, Color.Transparent);
            }

            using (var region = new Region(bounds))
            {
                graphics.ExcludeClip(region);
            }
        }

        private PropertyInfo _isValueEditablePropertyInfo;

        private bool IsValueEditable(object context)
        {
            if (_isValueEditablePropertyInfo == null)
            {
                _isValueEditablePropertyInfo = context.GetType().GetProperty("IsValueEditable");
            }

            return _isValueEditablePropertyInfo == null || (bool)_isValueEditablePropertyInfo.GetValue(context, null);
        }
    }
}
