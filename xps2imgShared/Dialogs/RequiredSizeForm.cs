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

            paperTypeIntControl.Objects = new [] { "A0", "A1", "A2", "A3", "A4", "A5", "A6", "B0", "B1", "B2", "B3", "B4", "B5", "B6", "C0", "C1", "C2", "C3", "C4", "C5", "C6" };
            paperTypeIntControl.TrackBarLargeChange = 7; // Num of papers.
            paperTypeIntControl.Value = 4; // A4
        }

        public Size? Value { get; set; }
    }
}
