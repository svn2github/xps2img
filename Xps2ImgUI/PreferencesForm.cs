using System;
using System.ComponentModel;
using System.Windows.Forms;

using CommandLine;

using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class PreferencesForm : Form
    {
        private readonly bool _isRunning;

        public PreferencesForm(Preferences preferences, bool isRunning)
        {
            InitializeComponent();

            _isRunning = isRunning;

            Preferences = preferences;
        }

        protected override void OnLoad(EventArgs e)
        {
            this.RemoveSystemMenuDisabledItems();

            preferencesPropertyGrid.ResetGroupCallback = PropertyGridResetGroupCallback;

            preferencesPropertyGrid.ModernLook = !Preferences.ClassicLook;
            preferencesPropertyGrid.RemoveLastToolStripItem();

            _resetToolStripButton = preferencesPropertyGrid.AddToolStripButton(Resources.Strings.ResetToDefault, ResetToolStripButtonClick);

            preferencesPropertyGrid.DocLines = 5;
            preferencesPropertyGrid.MoveSplitterByPercent(50);

            preferencesPropertyGrid.SelectGridItem(Preferences.DefaultSelectedItem);

            ReflectionUtils.SetReadOnly<Preferences>(_isRunning, () => Preferences.ShortenExtension);

            EnableReset();

            base.OnLoad(e);
        }

        private bool PropertyGridResetGroupCallback(string label, bool check)
        {
            if (!check)
            {
                preferencesPropertyGrid.ResetByCategory(label,
                    pi => !_isRunning || pi.Name != ReflectionUtils.GetPropertyName(() => Preferences.ShortenExtension));
                EnableReset();
            }

            return true;
        }
        
        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            ShowHelp();
        }

        private void ResetToolStripButtonClick(object sender, EventArgs e)
        {
            var shortenExtension = Preferences.ShortenExtension;

            Preferences.Reset();

            if (_isRunning)
            {
                Preferences.ShortenExtension = shortenExtension;
            }

            preferencesPropertyGrid.Refresh();

            ((ToolStripButton)sender).Enabled = false;
        }

        private void PreferencesPropertyGridPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            EnableReset();
        }

        private void PreferencesFormHelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            ShowHelp();
        }

        public Preferences Preferences
        {
            get { return (Preferences)preferencesPropertyGrid.SelectedObject; }
            private set { preferencesPropertyGrid.SelectedObject = value.DeepClone(); }
        }

        private void EnableReset()
        {
            var enableReset = !Preferences.Default.Equals(Preferences);

            _resetToolStripButton.Enabled = enableReset;

            if (!enableReset)
            {
                preferencesPropertyGrid.Focus();
            }
        }

        private void ShowHelp()
        {
            Help.ShowHelp(this, Program.HelpFile, HelpNavigator.TopicId, Program.HelpTopicPreferences);           
        }

        private ToolStripButton _resetToolStripButton;
    }
}
