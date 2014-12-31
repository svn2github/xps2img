using System;

using Xps2ImgUI.Settings;

namespace Xps2ImgUI
{
    public partial class PreferencesForm
    {
        private class ChangesTracker
        {
            private readonly Preferences.Localizations _originalApplicationLanguage;
            private readonly bool _originalClassicLook;
            private readonly bool _originalAlwaysResume;

            private readonly PreferencesForm _preferencesForm;

            private Preferences Preferences
            {
                get { return _preferencesForm.Preferences; }
            }

            public ChangesTracker(PreferencesForm preferencesForm)
            {
                _preferencesForm = preferencesForm;

                _originalApplicationLanguage = Preferences.ApplicationLanguage;
                _originalClassicLook = Preferences.ClassicLook;
                _originalAlwaysResume = Preferences.AlwaysResume;
            }

            public void NotifyIfChanged(Action afterApplicationLanguageAction = null, bool resetAll = false)
            {
                if (Preferences.ApplicationLanguage != _originalApplicationLanguage)
                {
                    if (resetAll)
                    {
                        Preferences.ApplicationLanguage = _originalApplicationLanguage;
                    }
                    _preferencesForm.ChangeCulture();
                    (afterApplicationLanguageAction ?? delegate { })();
                }

                if (Preferences.ClassicLook != _originalClassicLook)
                {
                    if (resetAll)
                    {
                        Preferences.ClassicLook = _originalClassicLook;
                    }
                    _preferencesForm.ChangePropertyGridLook();
                }

                if (Preferences.AlwaysResume != _originalAlwaysResume)
                {
                    if (resetAll)
                    {
                        Preferences.AlwaysResume = _originalAlwaysResume;
                    }
                    _preferencesForm.ChangePropertyAlwaysResume();
                }
            }
        }
    }
}
