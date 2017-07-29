using System;

using Xps2Img.Shared.Enums;

using Xps2ImgUI.Settings;

namespace Xps2ImgUI
{
    public partial class PreferencesForm
    {
        private class ChangesTracker
        {
            private readonly LanguagesSupported _originalApplicationLanguage;

            private readonly bool _originalClassicLook;
            private readonly bool _originalAlwaysResume;
            private readonly bool _originalUseFullExePath;

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
                _originalUseFullExePath = Preferences.UseFullExePath;
            }

            public void NotifyIfChanged(Action afterApplicationLanguageAction = null, bool resetAll = false)
            {
                NotifyIfApplicationLanguageChanged(afterApplicationLanguageAction, resetAll);
                NotifyIfClassicLookChanged(resetAll);
                NotifyIfAlwaysResumeChanged(resetAll);
                NotifyIfUseFullExePathChanged(resetAll);
            }

            private void NotifyIfAlwaysResumeChanged(bool resetAll)
            {
                if (Preferences.AlwaysResume == _originalAlwaysResume)
                {
                    return;
                }

                if (resetAll)
                {
                    Preferences.AlwaysResume = _originalAlwaysResume;
                }

                _preferencesForm.ChangePropertyAlwaysResume();
            }

            private void NotifyIfClassicLookChanged(bool resetAll)
            {
                if (Preferences.ClassicLook == _originalClassicLook)
                {
                    return;
                }

                if (resetAll)
                {
                    Preferences.ClassicLook = _originalClassicLook;
                }

                _preferencesForm.ChangePropertyGridLook();
            }

            private void NotifyIfUseFullExePathChanged(bool resetAll)
            {
                if (Preferences.UseFullExePath == _originalUseFullExePath)
                {
                    return;
                }

                if (resetAll)
                {
                    Preferences.UseFullExePath = _originalUseFullExePath;
                }

                _preferencesForm.ChangePropertyUseFullExePath();
            }

            private void NotifyIfApplicationLanguageChanged(Action afterApplicationLanguageAction, bool resetAll)
            {
                if (Preferences.ApplicationLanguage == _originalApplicationLanguage)
                {
                    return;
                }

                if (resetAll)
                {
                    Preferences.ApplicationLanguage = _originalApplicationLanguage;
                }

                _preferencesForm.ChangeCulture();

                (afterApplicationLanguageAction ?? delegate { })();
            }
        }
    }
}
