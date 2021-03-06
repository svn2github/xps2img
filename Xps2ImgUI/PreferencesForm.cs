﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using CommandLine.Utils;

using Xps2Img.Shared.Enums;
using Xps2Img.Shared.Localization;
using Xps2Img.Shared.Localization.Forms;

using Xps2ImgLib.Utils.Disposables;

using Xps2ImgUI.Controls.PropertyGridEx.ToolStripEx;

using Xps2ImgUI.Model;
using Xps2ImgUI.Settings;
using Xps2ImgUI.Utils.UI;

namespace Xps2ImgUI
{
    public partial class PreferencesForm : Form, IFormLocalization
    {
        private readonly Action<Preferences> _classicLookChanged;
        private readonly Action<Preferences> _alwaysResumeChanged;
        private readonly Action<Preferences> _useFullExePathChanged;

        private readonly ChangesTracker _changesTracker;

        private readonly Preferences _originalPreferences;

        private readonly Xps2ImgModel _model;

        private ToolStripButton _resetToolStripButton;
        private ToolStripSplitButton _languageToolStripSplitButton;

        private bool IsRunning
        {
            get { return _model.IsStopPending || _model.IsRunning; }
        }

        public PreferencesForm(Preferences preferences, Xps2ImgModel model, Action<Preferences> classicLookChanged, Action<Preferences> alwaysResumeChanged, Action<Preferences> useFullExePathChanged)
        {
            InitializeComponent();

            _model = model;

            SubscribeToModelEvents();

            Preferences = _originalPreferences = preferences;

            _classicLookChanged = classicLookChanged ?? delegate { };
            _alwaysResumeChanged = alwaysResumeChanged ?? delegate { };
            _useFullExePathChanged = useFullExePathChanged ?? delegate { };

            _changesTracker = new ChangesTracker(this);

            this.EnableFormLocalization();
        }

        private void SubscribeToModelEvents()
        {
            _model.Completed += ModelEvent;
            _model.LaunchFailed += ModelEvent;
            _model.LaunchSucceeded += ModelEvent;
        }

        private void UnsubscribeFromModelEvents()
        {
            _model.Completed -= ModelEvent;
            _model.LaunchFailed -= ModelEvent;
            _model.LaunchSucceeded -= ModelEvent;
        }

        private void ModelEvent(object sender, EventArgs eventArgs)
        {
            SetReadOnly();
            EnableButtons();
        }

        private void SetReadOnly()
        {
            ReflectionUtils.SetReadOnly<Preferences>(IsRunning, Preferences.Properties.ShortenExtension);
            _languageToolStripSplitButton.Enabled = !IsRunning;

            preferencesPropertyGrid.Refresh();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.RemoveSystemMenuDisabledItems();

            preferencesPropertyGrid.ResetGroupCallback = PropertyGridResetGroupCallback;
            preferencesPropertyGrid.ResetAllAction = ResetAll;
            preferencesPropertyGrid.ResetByCategoryFilter = pi => pi.Name != Preferences.Properties.ApplicationLanguage && (!IsRunning || pi.Name != Preferences.Properties.ShortenExtension);

            preferencesPropertyGrid.ModernLook = !Preferences.ClassicLook;
            preferencesPropertyGrid.RemoveLastToolStripItem();

            // Reset.
            _resetToolStripButton = preferencesPropertyGrid.AddToolStripButton(Resources.Images.Eraser, () => Resources.Strings.ResetToDefaults, (_, __) => preferencesPropertyGrid.ResetAllAction());

            // Languages.
            _languageToolStripSplitButton = preferencesPropertyGrid.AddToolStripSplitButton(
                () => GetLanguageName(Preferences.ApplicationLanguage), () => Resources.Strings.Preferences_ApplicationLanguageName, () => GetLanguageImage(Preferences.ApplicationLanguage),
                (sender, _) =>
                {
                    var toolStripSplitButton = (ToolStripSplitButton)sender;
                    UpdateLanguagesList(toolStripSplitButton);
                    toolStripSplitButton.DropDown.Show();
                });
            _languageToolStripSplitButton.Alignment = ToolStripItemAlignment.Right;
            _languageToolStripSplitButton.DropDownOpening += (_, __) => UpdateLanguagesList(_languageToolStripSplitButton);

            preferencesPropertyGrid.DocLines = 5;
            preferencesPropertyGrid.MoveSplitterByPercent(50);

            preferencesPropertyGrid.ExpandAllGridItems();
            preferencesPropertyGrid.SelectFirstGridItem(focusEdit: false);

            SetReadOnly();

            EnableButtons();

            preferencesPropertyGrid.PropertySort = _preferencesPropertySort;

            base.OnLoad(e);
        }

        private static string GetLanguageName(LanguagesSupported languageSupported)
        {
            return Xps2Img.Shared.Resources.Strings.ResourceManager.GetString(String.Format("LanguagesSupported_{0}Value", languageSupported));
        }

        private static Image GetLanguageImage(LanguagesSupported languageSupported)
        {
            return (Image)Resources.Images.ResourceManager.GetObject(languageSupported.ToString());
        }

        private void UpdateLanguagesList(ToolStripDropDownItem toolStripSplitButton)
        {
            toolStripSplitButton.DropDown.Items.Clear();

            foreach (var languageSupported in Enum.GetValues(typeof(LanguagesSupported)).Cast<LanguagesSupported>().Where(ls => ls != Preferences.ApplicationLanguage))
            {
                var languageSupportedLocal = languageSupported;
                var toolStripMenuItemEx = new ToolStripMenuItemEx(() => GetLanguageName(languageSupportedLocal), updateImage: () => GetLanguageImage(languageSupportedLocal));
                toolStripMenuItemEx.Click += (_, __) =>
                {
                    Preferences.ApplicationLanguage = languageSupportedLocal;
                    ChangeCulture();
                    EnableButtons();
                };

                toolStripSplitButton.DropDown.Items.Add(toolStripMenuItemEx);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            UnsubscribeFromModelEvents();

            if (DialogResult != DialogResult.Cancel)
            {
                return;
            }

            _changesTracker.NotifyIfChanged(resetAll: true);
        }

        private bool PropertyGridResetGroupCallback(string label, bool check)
        {
            if (check)
            {
                return preferencesPropertyGrid.IsResetByCategoryEnabled(label);
            }

            var changesTracker = new ChangesTracker(this);

            using (new DisposableAction(() => changesTracker.NotifyIfChanged(() => preferencesPropertyGrid.SelectGridItem(Resources.Strings.Preferences_InterfaceCategory))))
            {
                preferencesPropertyGrid.ResetByCategory(label);
            }

            EnableButtons();

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

            okButton.Text = Xps2Img.Shared.Resources.Strings.OK;
            cancelButton.Text = Xps2Img.Shared.Resources.Strings.Cancel;

            preferencesPropertyGrid.RefreshLocalization();
        }

        private void ResetAll()
        {
            var oldShortenExtension = Preferences.ShortenExtension;

            var changesTracker = new ChangesTracker(this);

            using (new DisposableAction(() => changesTracker.NotifyIfChanged(() => preferencesPropertyGrid.UpdateToolStripToolTip())))
            {
                Preferences.Reset(pi => pi.Name != Preferences.Properties.ApplicationLanguage);

                if (IsRunning)
                {
                    Preferences.ShortenExtension = oldShortenExtension;
                }
            }

            preferencesPropertyGrid.Refresh();

            _resetToolStripButton.Enabled = false;

            EnableOK();
        }

        private void PreferencesPropertyGridPropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // ReSharper disable once PossibleNullReferenceException
            var name = e.ChangedItem.PropertyDescriptor.Name;

            if (name == Preferences.Properties.ClassicLook)
            {
                ChangePropertyGridLook();
            }

            if (name == Preferences.Properties.AlwaysResume)
            {
                ChangePropertyAlwaysResume();
            }

            if (name == Preferences.Properties.UseFullExePath)
            {
                ChangePropertyUseFullExePath();
            }

            EnableButtons();
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

        private void ChangePropertyUseFullExePath()
        {
            _useFullExePathChanged(Preferences);
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

        private PropertySort _preferencesPropertySort;

        public PropertySort PreferencesPropertySort
        {
            get { return preferencesPropertyGrid.PropertySort; }
            set { _preferencesPropertySort = value != PropertySort.NoSort ? value : PreferencesPropertySort; }
        }

        private bool PreferencesDifferFrom(Preferences preferences)
        {
            return !Preferences.Equals(preferences, IsRunning);
        }

        // ReSharper disable once InconsistentNaming
        private void EnableOK()
        {
            okButton.Enabled = PreferencesDifferFrom(_originalPreferences);
        }

        private void EnableReset()
        {
            var enableReset = preferencesPropertyGrid.IsResetAllEnabled();

            _resetToolStripButton.Enabled = preferencesPropertyGrid.IsResetAllEnabled();

            if (!enableReset)
            {
                preferencesPropertyGrid.Focus();
            }
        }

        private void EnableButtons()
        {
            EnableOK();
            EnableReset();
        }

        private void ShowHelp()
        {
            if (!HelpUtils.ShowPropertyHelp(preferencesPropertyGrid))
            {
                HelpUtils.ShowHelpTopicId(HelpUtils.HelpTopicPreferences);
            }
        }
    }
}
