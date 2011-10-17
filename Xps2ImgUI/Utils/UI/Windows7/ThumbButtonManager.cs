using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming

namespace Windows7.DesktopIntegration
{
    public class ThumbButtonManager
    {
        public ThumbButtonManager(IntPtr hwnd)
        {
            _hwnd = hwnd;
        }

        public ThumbButton CreateThumbButton(Icon icon, string tooltip, EventHandler clickEventHandler)
        {
            return CreateThumbButton(_currentButtonId++, icon, tooltip, clickEventHandler);
        }

        public ThumbButton CreateThumbButton(uint id, Icon icon, string tooltip, EventHandler clickEventHandler)
        {
            return new ThumbButton(this, id, icon, tooltip, clickEventHandler);
        }

        public void AddThumbButtons(params ThumbButton[] buttons)
        {
            Array.ForEach(buttons, b => _thumbButtons.Add(b.Id, b));
            RefreshThumbButtons();
        }

        public void RefreshThumbButtons()
        {
            try
            {
                var win32Buttons = _thumbButtons.Values.Select(thumbButton => thumbButton.Win32ThumbButton).ToArray();
                if (_buttonsLoaded)
                {
                    Windows7Taskbar.TaskbarList.ThumbBarUpdateButtons(_hwnd, (uint) win32Buttons.Length, win32Buttons);
                }
                else
                {
                    Windows7Taskbar.TaskbarList.ThumbBarAddButtons(_hwnd, (uint) win32Buttons.Length, win32Buttons);
                    _buttonsLoaded = true;
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
            }
        }

        public void DispatchMessage(ref Message message)
        {
            var wparam = (UInt64)message.WParam.ToInt64();
            var wparam32 = (UInt32)(wparam & 0xffffffff);   //Clear top 32 bits

            if (message.Msg == WM_COMMAND && (wparam32 >> 16 == THBN_CLICKED))
            {
                _thumbButtons[wparam32 & 0xffff].FireClick();
            }
        }

        private bool _buttonsLoaded;
        private readonly Dictionary<uint, ThumbButton> _thumbButtons = new Dictionary<uint, ThumbButton>();
        private readonly IntPtr _hwnd;

        private uint _currentButtonId = 100;

        private const int WM_COMMAND = 0x111;
        private const uint THBN_CLICKED = 0x1800;
    }
}