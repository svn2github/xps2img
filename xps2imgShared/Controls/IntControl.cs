﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Xps2Img.Shared.Controls
{
    public partial class IntControl : UserControl
    {
        [Browsable(false)]
        public int DefaultValue { get; set; }

        [Browsable(false)]
        public int? Value { get; set; }

        [Browsable(false)]
        public int MinValue { get; set; }

        [Browsable(false)]
        public int MaxValue { get; set; }

        [Browsable(false)]
        public int TrackBarLargeChange { get; set; }

        [Browsable(false)]
        public int TrackBarTickFrequency { get; set; }

        [Browsable(false)]
        public string Title { get; set; }

        [Browsable(false)]
        public int[] Values { get; set; }

        [Browsable(false)]
        public int SelectedValue
        {
            get { return valueTrackBar.Value; }
            set { valueTrackBar.Value = value; }
        }

        public IntControl()
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
