using System.Drawing;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls
{
    public class Border : Panel
    {
        public Border()
        {
            var borderSize = SystemInformation.BorderSize;
            Padding = new Padding(borderSize.Width, borderSize.Height, borderSize.Width, borderSize.Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, SystemColors.ControlDark, ButtonBorderStyle.Solid);
        }
    }
}
