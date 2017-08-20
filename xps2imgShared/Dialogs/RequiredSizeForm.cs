using System;
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

        private class PaperSize
        {
            public string Name { get; private set; }

            public double Width { get; private set; }
            public double Height { get; private set; }

            public PaperSize(string name, double width, double height)
            {
                Name = name;
                Width = width;
                Height = height;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private void InitializePaperTypeIntControl()
        {
            paperTypeIntControl.Title = Resources.Strings.PaperSize;

            PaperSize a4;

            paperTypeIntControl.Objects = new []
            {
                // US
                new PaperSize("Half",   5.5, 8.5),
                new PaperSize("Letter", 8.5, 11.0),
                new PaperSize("Legal",  8.5, 14.0),
                new PaperSize("Junior", 5.0, 8.0),
                new PaperSize("Ledger", 11.0, 17.0),

                // ISO A
                new PaperSize("4A0", 66.2, 93.6),
                new PaperSize("2A0", 46.8, 66.2),
                new PaperSize("A0",  33.1, 46.8),
                new PaperSize("A1",  23.4, 33.1),
                new PaperSize("A2",  16.5, 23.4),
                new PaperSize("A3",  11.7, 16.5),
                a4 = new PaperSize("A4",  8.3,  11.7),
                new PaperSize("A5",  5.8,  8.3),
                new PaperSize("A6",  4.1,  5.8),
                new PaperSize("A7",  2.9,  4.1),
                new PaperSize("A8",  2.0,  2.9),
                new PaperSize("A9",  1.5,  2.0),
                new PaperSize("A10", 1.0,  1.5),

                // ISO B
                new PaperSize("B0",  39.4, 55.7),
                new PaperSize("B1",  27.8, 39.4),
                new PaperSize("B2",  19.7, 27.8),
                new PaperSize("B3",  13.9, 19.7),
                new PaperSize("B4",  9.8,  13.9),
                new PaperSize("B5",  6.9,  9.8),
                new PaperSize("B6",  4.9,  6.9),
                new PaperSize("B7",  3.5,  4.9),
                new PaperSize("B8",  2.4,  3.5),
                new PaperSize("B9",  1.7,  2.4),
                new PaperSize("B10", 1.2,  1.7),

                // ISO C
                new PaperSize("C0",  36.1, 51.5),
                new PaperSize("C1",  25.5, 36.1),
                new PaperSize("C2",  18.0, 25.5),
                new PaperSize("C3",  12.8, 18.0),
                new PaperSize("C4",  9.0,  12.8),
                new PaperSize("C5",  6.4,  9.0),
                new PaperSize("C6",  4.5,  6.4),
                new PaperSize("C7",  3.2,  4.5),
                new PaperSize("C8",  2.2,  3.2),
                new PaperSize("C9",  1.6,  2.2),
                new PaperSize("C10", 1.1,  1.6),

                // ANSI
                new PaperSize("ANSI A", 8.5, 11.0),
                new PaperSize("ANSI B", 11.0, 17.0),
                new PaperSize("ANSI C", 17.0, 22.0),
                new PaperSize("ANSI D", 22.0, 34.0),
                new PaperSize("ANSI E", 34.0, 44.0),

                // Architectural
                new PaperSize("Arch A",  9.0,  12.0),
                new PaperSize("Arch B",  12.0, 18.0),
                new PaperSize("Arch C",  18.0, 24.0),
                new PaperSize("Arch D",  24.0, 36.0),
                new PaperSize("Arch E",  36.0, 48.0),
                new PaperSize("Arch E1", 30.0, 42.0),

                // Paper Stock Type Uncut
                new PaperSize("Bond",      22.0, 17.0),
                new PaperSize("Book",      38.0, 25.0),
                new PaperSize("Cover",     26.0, 20.0),
                new PaperSize("Index",     30.5, 25.5),
                new PaperSize("Newsprint", 36.0, 24.0),
                new PaperSize("Offset",    38.0, 25.0),
                new PaperSize("Text",      38.0, 25.0),
                new PaperSize("Tissue",    36.0, 24.0),

                // Newspaper
                new PaperSize("Broadsheet",   23.5, 29.5),
                new PaperSize("Berliner",     12.4, 18.5),
                new PaperSize("Tabloid Size", 11.0, 16.9),

                // International Postcard
                new PaperSize("Postcard Max", 9.25, 4.72),
                new PaperSize("Postcard Min", 5.51, 3.54),

                // US Postcard
                new PaperSize("US Postcard Max", 6.0, 4.25),
                new PaperSize("US Postcard Min", 5.0, 3.5)
            };

            paperTypeIntControl.TrackBarLargeChange = 1;
            paperTypeIntControl.Value = Array.IndexOf(paperTypeIntControl.Objects, a4);
        }

        public Size? Value { get; set; }
    }
}
