using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

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
            var graphics = e.Graphics;
            var bounds = e.Bounds;

            graphics.DrawImage((bool)e.Value ? Resources.Images.Checked : Resources.Images.Unchecked, bounds);

            using (var region = new Region(bounds))
            {
                graphics.ExcludeClip(region);
            }
        }
    }
}
