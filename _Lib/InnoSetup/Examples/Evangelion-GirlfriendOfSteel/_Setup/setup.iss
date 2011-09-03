#include "SetupCommon.iss"

#if !defined(ISM_OnFileStub) && !FileExists("..\_Output\SubLang.exe")
    #error Compile SubLangSetup.iss first
#endif

#define OutputBaseFilename	"Evangelion-GirlfriendOfSteelSetup"

#define AppName             "Evangelion-Girlfriend of Steel"

#define AppPublisher        "GAINAX"
#define AppPublisherURL     "http://www.gainax.co.jp/"

#define UrlOfficialSite     "http://www.generation-x.co.jp/soft/koutetu/"

#define LicenseFile         "..\..\_Common\Files\info.rtf"
#define InfoBeforeFile      "..\readme.rtf"

#if defined(ISM_OnFileStub) && !FileExists(InfoBeforeFile)
    #undef InfoBeforeFile
#endif

#define AppReadmeFile       "{app}\readme.rtf"

#include "../../_Common/Setup.isi"

#include "Files.iss"
#include "Code.iss"

#define Active_AfterInstall  "CreateINI"
    <FileStub(EvaIniFile)>
<Reset_ActiveAfterInstall()>

#include "SubLang.iss"

<IconWalkthrough()>

<IconRun(name="{group}\{cm:Menu_SubtitlesLanguage}", filename="{app}\EXEC\SubLang.exe", comment="{cm:Menu_SubtitlesLanguage}")>

<IconUrlWeb("http://www.gainaxpages.com/", "{cm:Menu_WebGainaxPages}")>

<CustomMessage("en.Menu_Bonus", "Bonus Materials")>
<CustomMessage("en.Menu_BonusComment", "Browse " + AppName + " Bonus Materials")>

<CustomMessage("en.Menu_Endings", AppName + " Endings")>
<CustomMessage("en.Menu_ViewEndings", "View " + AppName + " Endings")>

<CustomMessage("en.Menu_WebGainaxPages", "Gainax Pages")>

<CustomMessage("en.Menu_SubtitlesLanguage", AppName + " Subtitles Language Selector")>

<Icon(name="{group}\{cm:Menu_Bonus}", filename="{app}\Bonus", comment="{cm:Menu_BonusComment}")>
<Icon(name="{group}\{cm:Folder_Cheats}\{cm:Menu_Endings}", filename="{app}\Cheats\Endings.html", comment="{cm:Menu_ViewEndings}")>

<Reg(key="HKCU\Software\GAINAX")>
<Reg(key="HKCU\Software\GAINAX\EVANGELION", flags=RegFlag_UninsDeleteKey)>
<Reg(EvaInstallRegPath, EvaInstallRegValue + ":string", "{app}")>

<Debug_ViewTranslation>

