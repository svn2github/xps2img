#ifndef __SUB_LANG_ISS__
#define __SUB_LANG_ISS__

#include "Code.iss"

; File stub is required to perform AfterInstall action.
#define LanguageSelection(lang, langName, checked=False)  \
    Task(Active_Tasks, langName, "{cm:Lang_Subtitles}", "", TaskFlag_Exclusive + (checked ? "" : TaskFlag_Unchecked)) + \
    FileStub("{tmp}\Languages", FileFlag_DeleteAfterInstall)

#define Active_Tasks            "Lang_En"
#define Active_AfterInstall     "SetLang('en')"
    <LanguageSelection("en", "{cm:Lang_English}", True)>
#define Active_Tasks            "Lang_Ru"
#define Active_AfterInstall     "SetLang('ru')"
    <LanguageSelection("ru", "{cm:Lang_Russian}")>
<Reset_ActiveTasks()>
<Reset_ActiveAfterInstall()>

<CustomMessage("en.Lang_Subtitles", "Subtitles language:")>
<CustomMessage("en.Lang_English", "English")>
<CustomMessage("en.Lang_Russian", "Русский")>

#endif

