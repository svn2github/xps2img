using System;
using System.Drawing;

namespace Xps2ImgUI.Settings
{
    [Serializable]
    public class FormState
    {
        public bool Maximized { get; set; }
        public Point Location { get; set; }
        public Size Size { get; set; }

        public bool IsEmpty
        {
            get { return Size.Width <= 0 || Size.Height <= 0; }
        }
    }
}
