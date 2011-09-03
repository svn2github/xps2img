#include "../../../_Common/Config.isi"

#define DIR_LEVEL	"../"

#define OutputBaseFilename		"ThiefDeadlyShadowsSetup"

#define AppName              	"Thief - Deadly Shadows"
#define AppExe					"System\T3.exe"
#define AppVersion				"1.1"

#define SetupVersion			AppVersion + ".0.0"

#define AppPublisher			"Eidos Interactive, Ltd."
#define AppPublisherURL			"http://www.eidos.com/"
#define AppSupportURL			"http://www.eidosinteractive.com/support/"

#define LicenseFile				"../EULA.rtf"
#define AppReadmeFile			"{app}\readme.rtf"

#define UseDiskSpanning

#define DirectXVersion			"9"

#define	OnInitializeSetup		"CheckOS()"

#include "Code.iss"

#include "../../../_Common/Setup.isi"

#include "Files.iss"

<Reg("HKLM\SOFTWARE\Ion Storm")>
<Reg("HKLM\SOFTWARE\Ion Storm\Thief - Deadly Shadows")>
<Reg("HKLM\SOFTWARE\Ion Storm\Thief - Deadly Shadows\1.0")>
<Reg("HKLM\SOFTWARE\Ion Storm\Thief - Deadly Shadows", "ION_ROOT:string", "{app}")>
<Reg("HKLM\SOFTWARE\Ion Storm\Thief - Deadly Shadows", "SaveGamePath:string", "{app}")>

<CustomMessage("en.Message_NotSupportedOSes", "Your Windows version is not supported.%n%nThief: Deadly Shadows is not supported on Windows NT 4.0, Windows 98, Windows Millennium Edition, and Windows Server 2003.%n%nWould you like to continue installation anyway?")>

<CustomMessage("en.Menu_EidosInteractive",	"Eidos Interactive")>
<CustomMessage("en.Menu_EidosOnlineStore",	"Eidos Online Store")>
<CustomMessage("en.Menu_ThiefDSEidosForums","Thief - Deadly Shadows - Eidos Forums")>
<CustomMessage("en.Menu_ThiefDSTweakGuide",	"Thief - Deadly Shadows Tweak Guide")>

<IconFaq()>
<IconWalkthrough()>
<IconHints()>

<IconUrlWeb("http://forums.eidosgames.com/forumdisplay.php?s=&forumid=46", "{cm:Menu_ThiefDSEidosForums}")>
<IconUrlWeb("http://www.tweakguides.com/TDS_1.html", "{cm:Menu_ThiefDSTweakGuide}")>
<IconUrlWeb("http://www.eidos.com/", "{cm:Menu_EidosInteractive}")>
<IconUrlWeb("http://www.eidosstore.com/", "{cm:Menu_EidosOnlineStore}")>

<Dir("{app}\SaveGames")>
<Dir("{app}\CONTENT\T3\Books\content")>
<Dir("{app}\CONTENT\T3\Books\English\SplashTexts")>

<UninstallDelete("{userdocs}\" + AppName + "\Launcher.log")>
<UninstallDelete("{userdocs}\" + AppName + "}", DeleteFlag_DirIfEmpty)>

<Debug_ViewTranslation>

