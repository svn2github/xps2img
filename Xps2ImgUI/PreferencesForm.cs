using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

using CommandLine.Utils;

using Xps2ImgUI.Localization;
using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class PreferencesForm : Form, IFormLocalization
    {
        private readonly bool _isRunning;

        private readonly Preferences.Localizations _originalApplicationLanguage;
        private readonly bool _originalClassicLook;
        private readonly bool _originalAlwaysResume;

        private readonly Action<Preferences> _classicLookChanged;
        private readonly Action<Preferences> _alwaysResumeChanged;

        public PreferencesForm(Preferences preferences, bool isRunning, Action<Preferences> classicLookChanged = null, Action<Preferences> alwaysResumeChanged = null)
        {
            InitializeComponent();

            _isRunning = isRunning;

            _originalApplicationLanguage = preferences.ApplicationLanguage;
            _originalClassicLook = preferences.ClassicLook;
            _originalAlwaysResume = preferences.AlwaysResume;

            Preferences = preferences;

            _classicLookChanged = classicLookChanged ?? delegate { };
            _alwaysResumeChanged = alwaysResumeChanged ?? delegate { };

            this.EnableFormLocalization();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.RemoveSystemMenuDisabledItems();

            preferencesPropertyGrid.ResetGroupCallback = PropertyGridResetGroupCallback;

            preferencesPropertyGrid.ModernLook = !Preferences.ClassicLook;
            preferencesPropertyGrid.RemoveLastToolStripItem();

            _resetToolStripButton = preferencesPropertyGrid.AddToolStripButton(Resources.Images.Eraser, () => Resources.Strings.ResetToDefaults, ResetToolStripButtonClick);

            preferencesPropertyGrid.DocLines = 5;
            preferencesPropertyGrid.MoveSplitterByPercent(50);

            preferencesPropertyGrid.SelectFirstGridItem();

            ReflectionUtils.SetReadOnly<Preferences>(_isRunning, Preferences.Properties.ShortenExtension);
            ReflectionUtils.SetReadOnly<Preferences>(_isRunning, Preferences.Properties.ApplicationLanguage);

            EnableReset();

            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Reset language on cancel.
            if (DialogResult != DialogResult.Cancel)
            {
                return;
            }

            if(Preferences.ApplicationLanguage != _originalApplicationLanguage)
            {
                Preferences.ApplicationLanguage = _originalApplicationLanguage;
                ChangeCulture();
            }

            if(Preferences.ClassicLook != _originalClassicLook)
            {
                Preferences.ClassicLook = _originalClassicLook;
                ChangePropertyGridLook();
            }

            if (Preferences.AlwaysResume != _originalAlwaysResume)
            {
                Preferences.AlwaysResume = _originalAlwaysResume;
                ChangePropertyAlwaysResume();
            }
        }

        private bool PropertyGridResetGroupCallback(string label, bool check)
        {
            Func<PropertyInfo, bool> allowFilter = pi => !_isRunning || (pi.Name != Preferences.Properties.ShortenExtension && pi.Name != Preferences.Properties.ApplicationLanguage);

            if (check)
            {
                return preferencesPropertyGrid.IsResetByCategoryEnabled(label, allowFilter);
            }

            var oldApplicationLanguage = Preferences.ApplicationLanguage;
            var oldClassicLook = Preferences.ClassicLook;
            var oldAlwaysResume = Preferences.AlwaysResume;

            preferencesPropertyGrid.ResetByCategory(label, allowFilter);

            if (oldApplicationLanguage != Preferences.ApplicationLanguage)
            {
                ChangeCulture();
                preferencesPropertyGrid.SelectGridItem(Resources.Strings.Preferences_GeneralCategory);
            }

            if (oldClassicLook != Preferences.ClassicLook)
            {
                ChangePropertyGridLook();
            }

            if (oldAlwaysResume != Preferences.AlwaysResume)
            {
                ChangePropertyAlwaysResume();
            }

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
            var oldClassicLook = Preferences.ClassicLook;
            var oldAlwaysResume = Preferences.AlwaysResume;

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
                preferencesPropertyGrid.UpdateToolStripToolTip();
            }

            if (oldClassicLook != Preferences.ClassicLook)
            {
                ChangePropertyGridLook();
            }

            if (oldAlwaysResume != Preferences.AlwaysResume)
            {
                ChangePropertyAlwaysResume();
            }

            preferencesPropertyGrid.Refresh();

            ((ToolStripButton)sender).Enabled = false;
        }

        private void PreferencesPropertyGridPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // ReSharper disable once PossibleNullReferenceException
            var name = e.ChangedItem.PropertyDescriptor.Name;

            if (name == Preferences.Properties.ApplicationLanguage)
            {
                ChangeCulture();
            }

            if (name == Preferences.Properties.ClassicLook)
            {
                ChangePropertyGridLook();
            }

            if (name == Preferences.Properties.AlwaysResume)
            {
                ChangePropertyAlwaysResume();
            }

            EnableReset();
        }

        private void ChangePropertyGridLook()
        {
            preferencesPropertyGrid.ModernLook = !Preferences.ClassicLook;
            _classicLookChanged(Preferences);
        }

        private void ChangePropertyAlwaysResume()
        {
            _alwaysResumeChanged(Preferences);
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
            if (!HelpUtils.ShowPropertyHelp(preferencesPropertyGrid))
            {
                HelpUtils.ShowHelpTopicId(HelpUtils.HelpTopicPreferences);
            }
        }

        private ToolStripButton _resetToolStripButton;
    }
}
