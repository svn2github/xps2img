using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

using CommandLine;

using Xps2ImgUI.Localization;
using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class PreferencesForm : Form, IFormLocalization
    {
        private readonly bool _isRunning;

        private readonly string _shortenExtensionPropertyName;
        private readonly string _applicationLanguagePropertyName;

        public PreferencesForm(Preferences preferences, bool isRunning)
        {
            InitializeComponent();

            _isRunning = isRunning;

            Preferences = preferences;

            _shortenExtensionPropertyName = ReflectionUtils.GetPropertyName(() => Preferences.ShortenExtension);
            _applicationLanguagePropertyName = ReflectionUtils.GetPropertyName(() => Preferences.ApplicationLanguage);

            this.EnableFormLocalization();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.RemoveSystemMenuDisabledItems();

            preferencesPropertyGrid.ResetGroupCallback = PropertyGridResetGroupCallback;

            preferencesPropertyGrid.ModernLook = !Preferences.ClassicLook;
            preferencesPropertyGrid.RemoveLastToolStripItem();

            _resetToolStripButton = preferencesPropertyGrid.AddToolStripButton(Resources.Images.Eraser, Resources.Strings.ResetToDefaults, ResetToolStripButtonClick);

            preferencesPropertyGrid.DocLines = 5;
            preferencesPropertyGrid.MoveSplitterByPercent(50);

            preferencesPropertyGrid.SelectGridItem(Preferences.DefaultSelectedItem);

            ReflectionUtils.SetReadOnly<Preferences>(_isRunning, _shortenExtensionPropertyName);
            ReflectionUtils.SetReadOnly<Preferences>(_isRunning, _applicationLanguagePropertyName);

            EnableReset();

            base.OnLoad(e);
        }

        private bool PropertyGridResetGroupCallback(string label, bool check)
        {
            Func<PropertyInfo, bool> allowFilter = pi => !_isRunning || (pi.Name != _shortenExtensionPropertyName && pi.Name != _applicationLanguagePropertyName);

            if (check)
            {
                return preferencesPropertyGrid.IsResetByCategoryEnabled(label, allowFilter);
            }

            preferencesPropertyGrid.ResetByCategory(label, allowFilter);

            EnableReset();

            return true;
        }
        
        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            hevent.Handled = true;
            ShowHelp();
        }

        public void UICultureChanged()
        {
            Text = Resources.Strings.Preferences_Title;

            okButton.Text = Resources.Strings.OK;
            cancelButton.Text = Resources.Strings.Cancel;

            preferencesPropertyGrid.RefreshLocalization();
        }

        private void ResetToolStripButtonClick(object sender, EventArgs e)
        {
            var oldShortenExtension = Preferences.ShortenExtension;
            var oldApplicationLanguage = Preferences.ApplicationLanguage;

            Preferences.Reset();

            var newApplicationLanguage = Preferences.ApplicationLanguage;

            if (_isRunning)
            {
                Preferences.ShortenExtension = oldShortenExtension;
                Preferences.ApplicationLanguage = oldApplicationLanguage;
            }

            if (oldApplicationLanguage != newApplicationLanguage)
            {
                ChangeCulture();
            }

            preferencesPropertyGrid.Refresh();

            ((ToolStripButton)sender).Enabled = false;
        }

        private void PreferencesPropertyGridPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if(e.ChangedItem.PropertyDescriptor.Name == _applicationLanguagePropertyName)
            {
                ChangeCulture();
            }

            EnableReset();
        }

        private void ChangeCulture()
        {
            LocalizationManager.SetUICulture((int)Preferences.ApplicationLanguage);
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
            if (!this.ShowPropertyHelp(preferencesPropertyGrid))
            {
                this.ShowHelpTopicId(HelpUtils.HelpTopicPreferences);
            }
        }

        private ToolStripButton _resetToolStripButton;
    }
}
