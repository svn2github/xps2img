using System;
using System.Linq;

namespace Xps2Img.Shared.Dialogs
{
    public partial class ListForm : BaseForm, IFormValue<int?>
    {
        public ListForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            valueComboBox.Text = (Value ?? DefaultValue).ToString();
            valueComboBox.Items.AddRange(Values.Select(v => v.ToString()).Cast<object>().ToArray());
            valueTrackBar.Minimum = MinValue;
            valueTrackBar.Maximum = MaxValue;
            valueTrackBar.Value = Value ?? DefaultValue;
            valueTrackBar.TickFrequency = TrackBarTickFrequency;
            valueTrackBar.LargeChange = TrackBarLargeChange;
            base.OnLoad(e);
        }

        public override void UICultureChanged()
        {
            if (IsHandleCreated)
            {
                headerLabel.Text = Title;
            }
            base.UICultureChanged();
        }

        public int DefaultValue { get; set; }
        public int? Value { get; set; }

        public int MaxValue { get; set; }
        public int MinValue { get; set; }

        public int TrackBarLargeChange { get; set; }
        public int TrackBarTickFrequency { get; set; }

        public string Title { get; set; }

        public int[] Values { get; set; }

        private void valueTrackBar_ValueChanged(object sender, EventArgs e)
        {
            valueComboBox.Text = valueTrackBar.Value.ToString();
        }
    }
}
