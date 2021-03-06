﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace Xps2Img.Shared.Controls
{
    public partial class IntControl : UserControl
    {
        private const string TrackingCategory = "Tracking";

        [Category("Control")]
        [Description("Hides track bar.")]
        [DefaultValue(false)]
        public bool HideTrackBar { get; set; }

        [Browsable(false)]
        [DefaultValue(0)]
        public int DefaultValue { get; set; }

        [Browsable(false)]
        [DefaultValue(null)]
        public int? Value { get; set; }

        [Browsable(false)]
        [DefaultValue(0)]
        public int MinValue { get; set; }

        [Browsable(false)]
        [DefaultValue(0)]
        public int MaxValue { get; set; }

        [Browsable(false)]
        [DefaultValue(0)]
        public int TrackBarLargeChange { get; set; }

        [Browsable(false)]
        [DefaultValue(0)]
        public int TrackBarTickFrequency { get; set; }

        [Browsable(false)]
        [DefaultValue(null)]
        public string Title { get; set; }

        [Browsable(false)]
        [DefaultValue(null)]
        public int[] Values { get; set; }

        private object[] _objects;

        [Browsable(false)]
        [DefaultValue(null)]
        public object[] Objects
        {
            get { return _objects; }
            set
            {
                if (value == null || !value.Any())
                {
                    return;
                }

                valueComboBox.DropDown -= ValueComboBoxDropDown;

                valueComboBox.DropDownStyle = HideTrackBar ? ComboBoxStyle.DropDownList : ComboBoxStyle.DropDown;

                valueComboBox.Items.Clear();
                valueComboBox.Items.AddRange(_objects = value);

                Values = Enumerable.Range(0, _objects.Length).ToArray();

                MinValue = Values.First();
                MaxValue = Values.Last();

                TrackBarTickFrequency = 1;               
            }
        }

        [Browsable(false)]
        [DefaultValue(0)]
        public int SelectedValue
        {
            get { return valueTrackBar.Value; }
            set { valueTrackBar.Value = value; }
        }

        private IntControl _alignTitleWidthWith;

        [Category(TrackingCategory)]
        [Description("Control title widths will be aligned to.")]
        [DefaultValue(null)]
        public IntControl AlignTitleWidthWith
        {
            get { return _alignTitleWidthWith; }
            set
            {
                if(value == this)
                {
                    throw new InvalidOperationException("Could not use self as assignment source. Choose another IntControl.");
                }
                _alignTitleWidthWith = value;
            }
        }

        private bool HasObjects { get { return Objects != null; } }

        public IntControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var value = Value ?? DefaultValue;
            if (!IsValueInRange(value))
            {
                value = MinValue;
            }

            valueTrackBar.Minimum = MinValue;
            valueTrackBar.Maximum = MaxValue;

            SetComboBoxValue(value);

            SelectedValue = value;

            valueTrackBar.TickFrequency = TrackBarTickFrequency;
            valueTrackBar.LargeChange = TrackBarLargeChange;

            headerLabel.Text = Title;

            if (AlignTitleWidthWith != null)
            {
                AlignTitleWidthWith.headerLabel.Resize += AlignTitleWidthWithHeaderLabelResize;
            }

            if (HideTrackBar)
            {
                valueComboBox.Anchor = AnchorStyles.Right | AnchorStyles.Left;
                valueComboBox.Size = new Size(valueTrackBar.Right - valueComboBox.Left, valueComboBox.Height);

                valueTrackBar.Visible = false;
            }

            AdjustValueComboBoxWidth();
        }

        private void SetComboBoxValue(int value)
        {
            if (HasObjects)
            {
                valueComboBox.SelectedIndex = AdjustValue(value);
            }
            else
            {
                valueComboBox.Text = IntToString(value);
            }
        }

        private void AlignTitleWidthWithHeaderLabelResize(object sender, EventArgs eventArgs)
        {
            var alignTitleWidthWithHeaderLabel = (Label)sender;

            alignTitleWidthWithHeaderLabel.Resize -= AlignTitleWidthWithHeaderLabelResize;

            var headerLabelShorter = headerLabel.Width < alignTitleWidthWithHeaderLabel.Width;
            var mainHeaderLabel = headerLabelShorter ? alignTitleWidthWithHeaderLabel : headerLabel;
            var adjustedHeaderLabel = headerLabelShorter ? headerLabel : alignTitleWidthWithHeaderLabel;

            adjustedHeaderLabel.AutoSize = false;
            adjustedHeaderLabel.Width = mainHeaderLabel.Width;
        }

        private void SetSelectedValueFromTrackBar()
        {
            if (!_fromComboBox)
            {
                SetComboBoxValue(SelectedValue);
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
            var isValueValid = Int32.TryParse(strValue, NumberStyles.None, CultureInfo.InvariantCulture, out intValue);
            return AdjustValue(intValue, isValueValid);
        }

        private int AdjustValue(int intValue, bool isValueValid = true)
        {
            return isValueValid && IsValueInRange(intValue) ? intValue : SelectedValue;
        }

        private void AdjustValueComboBoxWidth()
        {
            if (HideTrackBar || !HasObjects)
            {
                return;
            }

            using (var graphics = valueComboBox.CreateGraphics())
            {
                var size = Objects.Select(s => s.ToString()).Max(s => graphics.MeasureString(s, valueComboBox.Font).Width);
                var width = (int)size + 1 + SystemInformation.VerticalScrollBarWidth;
                if (width <= valueComboBox.Width)
                {
                    return;
                }

                var delta = width - valueComboBox.Width;

                valueComboBox.Width = width;

                valueTrackBar.Left  += delta;
                valueTrackBar.Width -= delta;
            }
        }

        private bool _fromComboBox;

        private void SetSelectedValueFromComboBox()
        {
            _fromComboBox = true;
            SelectedValue = HasObjects ? AdjustValue(valueComboBox.SelectedIndex) : AdjustValue(valueComboBox.Text);
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
