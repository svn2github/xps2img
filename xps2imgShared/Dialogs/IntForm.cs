namespace Xps2Img.Shared.Dialogs
{
    public partial class IntForm : BaseForm, IFormValue<int?>
    {
        public IntForm()
        {
            InitializeComponent();
        }

        protected override bool CanClose()
        {
            Value = intControl.SelectedValue;
            return true;
        }

        public int DefaultValue
        {
            set { intControl.DefaultValue = value; }
        }

        public int? Value
        {
            get { return intControl.Value; }
            set { intControl.Value = value; }
        }

        public int MinValue
        {
            set { intControl.MinValue = value; }
        }

        public int MaxValue
        {
            set { intControl.MaxValue = value; }
        }

        public int TrackBarLargeChange
        {
            set { intControl.TrackBarLargeChange = value; }
        }

        public int TrackBarTickFrequency
        {
            set { intControl.TrackBarTickFrequency = value; }
        }
        
        public string Title
        {
            set { intControl.Title = value; }
        }
        
        public int[] Values
        {
            set { intControl.Values = value; }
        }

        public object[] Objects
        {
            set { intControl.Objects = value; }
        }
    }
}
