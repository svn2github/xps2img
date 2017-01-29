using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;

using Xps2Img.Shared.Utils;

namespace Xps2Img.Shared.TypeEditors
{
    public class CheckBoxGlyphEditor : UITypeEditor
    {
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        private static Bitmap GetStateImage(object context, bool value)
        {
            var attributeCollection = ReflectionUtils.GetPropertyValue<AttributeCollection>(context, "Attributes");
            var defaultAttribute = attributeCollection != null ? attributeCollection.OfType<DefaultValueAttribute>().FirstOrDefault() : null;

            var grid = ReflectionUtils.GetPropertyValue(context, "OwnerGrid", false);
            var gridEdit = ReflectionUtils.GetPropertyValue(grid, "Editor", false);
            var filterOpened = ReflectionUtils.GetPropertyValue<bool>(gridEdit, "Filter", false);

            var defaultValue = filterOpened || (defaultAttribute != null && (bool)defaultAttribute.Value == value);

            return value
                     ? (defaultValue ? Resources.Images.CheckedDefault : Resources.Images.Checked)
                     : (defaultValue ? Resources.Images.UncheckedDefault : Resources.Images.Unchecked);
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            var bounds   = e.Bounds;
            var context  = e.Context;
            var graphics = e.Graphics;

            var stateImage = GetStateImage(context, (bool)e.Value);

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

        private static bool IsValueEditable(object context)
        {
            return ReflectionUtils.GetPropertyValue<bool?>(context, "IsValueEditable", false) ?? true;
        }
    }
}
