using System;
using System.Collections.Generic;
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
            var value = Value ?? DefaultValue;
            if (!IsValueInRange(value))
            {
                value = MinValue;
            }

            valueComboBox.Text = IntToString(value);

            valueTrackBar.Minimum = MinValue;
            valueTrackBar.Maximum = MaxValue;

            SelectedValue = value;

            valueTrackBar.TickFrequency = TrackBarTickFrequency;
            valueTrackBar.LargeChange = TrackBarLargeChange;

            headerLabel.Text = Title;

            base.OnLoad(e);
        }

        protected override bool CanClose()
        {
            Value = SelectedValue;
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

        private void SetSelectedValueFromTrackBar()
        {
            if (!_fromComboBox)
            {
                valueComboBox.Text = IntToString(SelectedValue);
            }
        }

        private void ValueTrackBarEnter(object sender, EventArgs e)
        {
            SetSelectedValueFromTrackBar();
        }

        private void ValueTrackBarValueChanged(object sender, EventArgs e)
        {
            SetSelectedValueFromTrackBar();
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
                     : SelectedValue;
        }

        private int SelectedValue
        {
            get { return valueTrackBar.Value; }
            set { valueTrackBar.Value = value; }
        }

        private bool _fromComboBox;

        private void SetSelectedValueFromComboBox()
        {
            _fromComboBox = true;
            SelectedValue = AdjustValue(valueComboBox.Text);
            _fromComboBox = false;
        }

        private void ValueComboBoxSelectedValueChanged(object sender, EventArgs e)
        {
            SetSelectedValueFromComboBox();
        }

        private void ValueComboBoxTextUpdate(object sender, EventArgs e)
        {
            SetSelectedValueFromComboBox();
        }

        private IEnumerable<object> GetValueComboBoxItems(int insertIndex, int value)
        {
            for (var i = 0; i < Values.Length; i++)
            {
                if (i == insertIndex)
                {
                    yield return value;
                }
                yield return Values[i];
            }
        }

        private void ValueComboBoxDropDown(object sender, EventArgs e)
        {
            var selectedValue = SelectedValue;
            var index = IsValueInRange(selectedValue) ? Array.BinarySearch(Values, selectedValue) : 0;
            index = index < 0 ? ~index : -1;

            valueComboBox.Items.Clear();
            valueComboBox.Items.AddRange(GetValueComboBoxItems(index, selectedValue).ToArray());
        }
    }
}
