using System;
using System.Drawing;

using Windows7.DesktopIntegration.Interop;

// ReSharper disable CheckNamespace

namespace Windows7.DesktopIntegration
{
    public class ThumbButton
    {
        private readonly ThumbButtonManager _manager;

        internal ThumbButton(ThumbButtonManager manager, uint id, Icon icon, string tooltip, EventHandler clickEventHandler)
        {
            _manager = manager;

            Id = id;
            Icon = icon;
            Tooltip = tooltip;

            Clicked += clickEventHandler;
        }

        public uint Id { get; set; }
        public Icon Icon { get; set; }
        public string Tooltip { get; set; }

        public bool Visible
        {
            get
            {
                return (Flags & THBFLAGS.THBF_HIDDEN) == 0;
            }
            set
            {
                if (value)
                {
                    Flags &= ~THBFLAGS.THBF_HIDDEN;
                }
                else
                {
                    Flags |= THBFLAGS.THBF_HIDDEN;
                }
                _manager.RefreshThumbButtons();
            }
        }

        public bool Enabled
        {
            get
            {
                return (Flags & THBFLAGS.THBF_DISABLED) == 0;
            }
            set
            {
                if (value)
                {
                    Flags &= ~(THBFLAGS.THBF_DISABLED);
                }
                else
                {
                    Flags |= THBFLAGS.THBF_DISABLED;
                }
                _manager.RefreshThumbButtons();
            }
        }

        public event EventHandler Clicked;

        internal void FireClick()
        {
            var clicked = Clicked;
            if (clicked != null)
            {
                clicked(this, EventArgs.Empty);
            }
        }

        private THBFLAGS Flags { get; set; }

        internal THUMBBUTTON Win32ThumbButton
        {
            get
            {
                var win32ThumbButton = new THUMBBUTTON
                                           {
                                               iId = Id,
                                               szTip = Tooltip,
                                               hIcon = Icon.Handle,
                                               dwFlags = Flags,
                                               dwMask = THBMASK.THB_FLAGS
                                           };

                if (Tooltip != null)
                    win32ThumbButton.dwMask |= THBMASK.THB_TOOLTIP;

                if (Icon != null)
                    win32ThumbButton.dwMask |= THBMASK.THB_ICON;

                return win32ThumbButton;
            }
        }
    }
}