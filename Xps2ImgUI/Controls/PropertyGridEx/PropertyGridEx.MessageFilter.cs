using System;
using System.Windows.Forms;

namespace Xps2ImgUI.Controls.PropertyGridEx
{
    public partial class PropertyGridEx
    {
        private class MessageFilter : IMessageFilter
        {
            // ReSharper disable once InconsistentNaming
            private const int WM_LBUTTONDBLCLK = 0x0203;

            private readonly Func<Message, bool> _shouldApplyFunc;
            private readonly Func<bool> _processLeftDoubleClickFunc;

            public MessageFilter(Func<Message, bool> shouldApplyFunc, Func<bool> processLeftDoubleClickFunc)
            {
                _shouldApplyFunc = shouldApplyFunc;
                _processLeftDoubleClickFunc = processLeftDoubleClickFunc;
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (!_shouldApplyFunc(m))
                {
                    return false;
                }

                return m.Msg == WM_LBUTTONDBLCLK && _processLeftDoubleClickFunc();
            }
        }
    }
}
