using System.ComponentModel;

using CommandLine;

namespace Xps2ImgUI.Settings
{
    public partial class Preferences
    {
        [Browsable(false)] public string PropNameApplicationLanguage { get { return ReflectionUtils.GetPropertyName(() => ApplicationLanguage); } }
        [Browsable(false)] public string PropNameClassicLook { get { return ReflectionUtils.GetPropertyName(() => ClassicLook); } }
        [Browsable(false)] public string PropNameAutoCompleteFilenames { get { return ReflectionUtils.GetPropertyName(() => AutoCompleteFilenames); } }
        [Browsable(false)] public string PropNameAutoSaveSettings { get { return ReflectionUtils.GetPropertyName(() => AutoSaveSettings); } }
        [Browsable(false)] public string PropNameFlashWhenCompleted { get { return ReflectionUtils.GetPropertyName(() => FlashWhenCompleted); } }
        [Browsable(false)] public string PropNameShowElapsedTimeAndStatistics { get { return ReflectionUtils.GetPropertyName(() => ShowElapsedTimeAndStatistics); } }
        [Browsable(false)] public string PropNameConfirmOnAfterConversion { get { return ReflectionUtils.GetPropertyName(() => ConfirmOnAfterConversion); } }
        [Browsable(false)] public string PropNameConfirmOnDelete { get { return ReflectionUtils.GetPropertyName(() => ConfirmOnDelete); } }
        [Browsable(false)] public string PropNameConfirmOnExit { get { return ReflectionUtils.GetPropertyName(() => ConfirmOnExit); } }
        [Browsable(false)] public string PropNameConfirmOnStop { get { return ReflectionUtils.GetPropertyName(() => ConfirmOnStop); } }
        [Browsable(false)] public string PropNameAlwaysResume { get { return ReflectionUtils.GetPropertyName(() => AlwaysResume); } }
        [Browsable(false)] public string PropNameSuggestResume { get { return ReflectionUtils.GetPropertyName(() => SuggestResume); } }
        [Browsable(false)] public string PropNameShortenExtension { get { return ReflectionUtils.GetPropertyName(() => ShortenExtension); } }
        [Browsable(false)] public string PropNameCheckForUpdates { get { return ReflectionUtils.GetPropertyName(() => CheckForUpdates); } }
    }
}
