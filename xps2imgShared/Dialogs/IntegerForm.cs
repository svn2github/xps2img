using System;
using System.Globalization;
using System.Linq;

namespace Xps2Img.Shared.Dialogs
{
    public partial class IntegerForm : BaseForm, IFormValue<int?>
    {
        public IntegerForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            valueComboBox.Text = IntToString(Value ?? DefaultValue);
            valueComboBox.Items.AddRange(Values.Select(v => IntToString(v)).Cast<object>().ToArray());

            valueTrackBar.Minimum = MinValue;
            valueTrackBar.Maximum = MaxValue;

            valueTrackBar.Value = Value ?? DefaultValue;

            valueTrackBar.TickFrequency = TrackBarTickFrequency;
            valueTrackBar.LargeChange = TrackBarLargeChange;

            headerLabel.Text = Title;

            base.OnLoad(e);
        }

        protected override bool CanClose(bool ok)
        {
            Value = valueTrackBar.Value;
            return true;
        }

        public int DefaultValue { get; set; }
        public int? Value { get; set; }

        public int MaxValue { get; set; }
        public int MinValue { get; set; }

        public int TrackBarLargeChange { get; set; }
        public int TrackBarTickFrequency { get; set; }

        public string Title { get; set; }

        public int[] Values { get; set; }

        private void ValueTrackBarValueChanged(object sender, EventArgs e)
        {
            if (!_fromComboBox)
            {
                valueComboBox.Text = IntToString(valueTrackBar.Value);
            }
        }

        private bool IsValueInRange(int intValue)
        {
            return intValue >= MinValue && intValue <= MaxValue;
        }

        private string IntToString(int? value)
        {
            return value.HasValue && IsValueInRange(value.Value) ? value.Value.ToString(CultureInfo.InvariantCulture) : String.Empty;
        }

        private int AdjustValue(string strValue)
        {
            int intValue;
            return Int32.TryParse(strValue, NumberStyles.None, CultureInfo.InvariantCulture, out intValue) && IsValueInRange(intValue)
                     ? intValue
                     : valueTrackBar.Value;
        }

        private bool _fromComboBox;

        private void ComboBoxValue()
        {
            _fromComboBox = true;
            valueTrackBar.Value = AdjustValue(valueComboBox.Text);
            _fromComboBox = false;
        }

        private void ValueComboBoxSelectedValueChanged(object sender, EventArgs e)
        {
            ComboBoxValue();
        }

        private void ValueComboBoxTextUpdate(object sender, EventArgs e)
        {
            ComboBoxValue();
        }
    }
}
