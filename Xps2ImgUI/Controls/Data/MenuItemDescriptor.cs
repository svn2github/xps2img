using System;

namespace Xps2ImgUI.Controls.Data
{
    public class MenuItemDescriptor
    {
        public Func<string> Title { get; private set; }
        public Action Command { get; private set; }

        public MenuItemDescriptor() : this(null, null)
        {
        }

        public MenuItemDescriptor(Func<string> title, Action command)
        {
            Title = title;
            Command = command;
        }

        public bool IsSeparator
        {
            get { return Title == null; }
        }

        public bool Enabled
        {
            get { return IsSeparator || Command != null; }
        }
    }
}