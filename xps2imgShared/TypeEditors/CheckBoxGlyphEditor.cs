using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Xps2Img.Shared.TypeEditors
{
    public class CheckBoxGlyphEditor : UITypeEditor
    {
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /*
         * 
         * 
GridEntryHost
OwnerGrid P
_propertyGridViewEdit
Filter P
         */

        private readonly Dictionary<string, PropertyInfo> _cachedProperties = new Dictionary<string, PropertyInfo>();

        private T GetPropertyValue<T>(object obj, string name, bool nonPublic = true)
        {
            PropertyInfo propertyInfo;

            if(!_cachedProperties.TryGetValue(name, out propertyInfo))
            {
                propertyInfo = obj.GetType().GetProperty(name, BindingFlags.Instance | (nonPublic ? BindingFlags.NonPublic : BindingFlags.Public));
                _cachedProperties.Add(name, propertyInfo);
            }

            return propertyInfo != null ? (T)propertyInfo.GetValue(obj, null) : default(T);
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            var bounds   = e.Bounds;
            var context  = e.Context;
            var graphics = e.Graphics;
            var value    = (bool)e.Value;

            var attributeCollection = GetPropertyValue<AttributeCollection>(context, "Attributes");
            var defaultAttribute = attributeCollection != null ? attributeCollection.OfType<DefaultValueAttribute>().FirstOrDefault() : null;
            var defaultValue = defaultAttribute != null && (bool)defaultAttribute.Value == value;

            var stateImage = value
                                ? (defaultValue ? Resources.Images.CheckedDefault   : Resources.Images.Checked)
                                : (defaultValue ? Resources.Images.UncheckedDefault : Resources.Images.Unchecked);

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

        private bool IsValueEditable(object context)
        {
            return GetPropertyValue<bool?>(context, "IsValueEditable", false) ?? true;
        }
    }
}
