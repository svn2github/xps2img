using System;
using System.Windows.Forms;

using CommandLine;

using Xps2Img.CommandLine;

using Xps2ImgUI.Model;
using Xps2ImgUI.Settings;

namespace Xps2ImgUI
{
    public partial class MainForm
    {
        [Serializable]
        public class Settings
        {
            public PropertySort PropertySort { get; set; }
            public bool ShowCommandLine { get; set; }
            public string CommandLine { get; set; }
            public Preferences Preferences { get; set; }
        }

        public bool CanSezialize
        {
            get { return _preferences.AutoSaveSettings; }
        }

        public object GetSettings()
        {
            return new Settings
            {
                PropertySort = settingsPropertyGrid.PropertySort,
                ShowCommandLine = IsCommandLineVisible,
                CommandLine = _xps2ImgModel.FormatCommandLine(Options.ExcludedOnSave),
                Preferences = _preferences
            };
        }

        public void SetSettings(object serialized)
        {
            var settings = (Settings)serialized;
            settingsPropertyGrid.PropertySort = settings.PropertySort;
            IsCommandLineVisible = settings.ShowCommandLine;
            _preferences = settings.Preferences ?? new Preferences();
            if (!String.IsNullOrEmpty(settings.CommandLine))
            {
                SetModel(new Xps2ImgModel(Parser.Parse<Options>(settings.CommandLine, true)));
            }
        }

        public Type GetSettingsType()
        {
            return typeof(Settings);
        }
    }
}
