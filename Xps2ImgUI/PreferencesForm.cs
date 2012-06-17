using System;
using System.ComponentModel;
using System.Windows.Forms;

using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class PreferencesForm : Form
    {
        public PreferencesForm(Preferences preferences)
        {
            InitializeComponent();

            Preferences = preferences;
        }

        protected override void OnLoad(EventArgs e)
        {
            this.RemoveSystemMenuDisabledItems();

            preferencesPropertyGrid.ModernLook = !Preferences.ClassicLook;
            preferencesPropertyGrid.RemoveLastToolStripItem();

            _resetToolStripButton = preferencesPropertyGrid.AddToolStripButton(Resources.Strings.ResetToDefault, ResetToolStripButtonClick);

            preferencesPropertyGrid.DocLines = 5;
            preferencesPropertyGrid.MoveSplitterByPercent(50);

            EnableReset();

            preferencesPropertyGrid.SelectGridItem(Preferences.DefaultSelectedItem);

            base.OnLoad(e);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            ShowHelp();
        }

        private void ResetToolStripButtonClick(object sender, EventArgs e)
        {
            Preferences.Reset();

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
