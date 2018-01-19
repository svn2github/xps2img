using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using CommandLine;

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
            public PropertySort PreferencesPropertySort { get; set; }
            public bool ShowCommandLine { get; set; }
            public string CommandLine { get; set; }
            public Preferences Preferences { get; set; }
            public FormState MainFormState { get; set; }
        }

        private static FormState GetFormState(Form form)
        {
            var bounds = form.WindowState == FormWindowState.Normal ? form.Bounds : form.RestoreBounds;

            return new FormState
            {
                Maximized = form.WindowState == FormWindowState.Maximized,
                Location  = bounds.Location,
                Size      = bounds.Size
            };
        }

        private static void SetFormState(Form form, FormState formState)
        {
            if (formState == null || formState.IsEmpty)
            {
                return;
            }

            var bounds = new Rectangle(formState.Location, formState.Size);

            if (!Screen.AllScreens.Any(screen => screen.WorkingArea.IntersectsWith(bounds)))
            {
                return;
            }

            form.StartPosition = FormStartPosition.Manual;

            if (formState.Maximized)
            {
                form.WindowState = FormWindowState.Maximized;
            }

            form.Bounds = bounds;
        }

        public object GetSettings()
        {
            return new Settings
            {
                PropertySort = settingsPropertyGrid.PropertySort,
                PreferencesPropertySort = _preferencesPropertySort,
                ShowCommandLine = IsCommandLineVisible,
                CommandLine = _preferences.AutoSaveSettings ? Model.FormatCommandLineForSave() : null,
                Preferences = _preferences,
                MainFormState = GetFormState(this)
            };
        }

        public void SetSettings(object serialized)
        {
            var settings = (Settings)serialized;
            settingsPropertyGrid.PropertySort = settings.PropertySort;
            _preferencesPropertySort = settings.PreferencesPropertySort;
            IsCommandLineVisible = settings.ShowCommandLine;
            _preferences = settings.Preferences ?? new Preferences();
            if (!String.IsNullOrEmpty(settings.CommandLine))
            {
                Model = new Xps2ImgModel(Parser.Parse<UIOptions>(settings.CommandLine, true));
            }
            SetFormState(this, settings.MainFormState);
        }

        public Type GetSettingsType()
        {
            return typeof(Settings);
        }

        private PropertySort _preferencesPropertySort;

        private Preferences _preferences = new Preferences();
    }
}
