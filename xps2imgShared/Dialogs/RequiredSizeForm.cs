using System.Drawing;

using Xps2Img.Shared.CommandLine;

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

            dpiIntControl.MinValue = Options.ValidationExpressions.MinDpiValue;
            dpiIntControl.MaxValue = Options.ValidationExpressions.MaxDpiValue;

            dpiIntControl.TrackBarTickFrequency = 72;
            dpiIntControl.TrackBarLargeChange = 72;

            dpiIntControl.Values = Options.Defaults.DpiValues;

            dpiIntControl.Value = Options.Defaults.DpiValue;
            dpiIntControl.VerticalPair = paperTypeIntControl;
        }

        private void InitializePaperTypeIntControl()
        {
            paperTypeIntControl.Title = Resources.Strings.PaperSize;

            paperTypeIntControl.MinValue = Options.ValidationExpressions.MinDpiValue;
            paperTypeIntControl.MaxValue = Options.ValidationExpressions.MaxDpiValue;

            paperTypeIntControl.TrackBarTickFrequency = 72;
            paperTypeIntControl.TrackBarLargeChange = 72;

            paperTypeIntControl.Values = Options.Defaults.DpiValues;

            paperTypeIntControl.Value = Options.Defaults.DpiValue;
        }

        public Size? Value { get; set; }
    }
}
