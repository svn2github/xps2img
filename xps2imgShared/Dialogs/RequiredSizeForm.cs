using System.Drawing;

namespace Xps2Img.Shared.Dialogs
{
    public partial class RequiredSizeForm : BaseForm, IFormValue<Size?>
    {
        public RequiredSizeForm()
        {
            InitializeComponent();

            InitializePaperTypeIntControl();
            InitializeDpiIntControl();
        }

        private void InitializeDpiIntControl()
        {
            dpiIntControl.Title = Resources.Strings.Options_DpiName;

            dpiIntControl.MinValue = Shared.Controls.Settings.Dpi.MinValue;
            dpiIntControl.MaxValue = Shared.Controls.Settings.Dpi.MaxValue;

            dpiIntControl.TrackBarTickFrequency = Shared.Controls.Settings.Dpi.TrackBarTickFrequency;
            dpiIntControl.TrackBarLargeChange = Shared.Controls.Settings.Dpi.TrackBarLargeChange;

            dpiIntControl.Values = Shared.Controls.Settings.Dpi.Values;
            dpiIntControl.Value = Shared.Controls.Settings.Dpi.DefaultValue;
        }

        private void InitializePaperTypeIntControl()
        {
            paperTypeIntControl.Title = Resources.Strings.PaperSize;

            paperTypeIntControl.MinValue = Shared.Controls.Settings.Dpi.MinValue;
            paperTypeIntControl.MaxValue = Shared.Controls.Settings.Dpi.MaxValue;

            paperTypeIntControl.TrackBarTickFrequency = Shared.Controls.Settings.Dpi.TrackBarTickFrequency;
            paperTypeIntControl.TrackBarLargeChange = Shared.Controls.Settings.Dpi.TrackBarLargeChange;

            paperTypeIntControl.Values = Shared.Controls.Settings.Dpi.Values;
            paperTypeIntControl.Value = Shared.Controls.Settings.Dpi.DefaultValue;
        }

        public Size? Value { get; set; }
    }
}
