#include "../../_Common/Config.isi"

#define OutputBaseFilename	"SystemShock2Setup"

#define AppName				"System Shock 2"
#define AppExe				"shock2.exe"
#define AppDirName			"System Shock 2"

#define AppVersion			"2.3"
#define AppSetupVersion		AppVersion + ".0.0"

#define LicenseFile			"../license.rtf"

#define AppPublisher		"Irrational Games"
#define AppPublisherURL		"http://www.irrationalgames.com"

#define AppReadmeFile		"{app}\readme.wri"
#define AppManualFile		"{app}\Manual\sshock2manual.pdf"

#define DirectXVersion		"6.1"

#include "../../_Common/Setup.isi"

#include "Code.iss"
#include "Files.iss"

#define InstallCfg	'{app}\install.cfg'

#define Active_AfterInstall "SaveInstallCfg('" + InstallCfg + "')"
	<FileStub(InstallCfg)>
<Reset_ActiveAfterInstall()>

#define SetupFlags_RegServer FileFlag_OnlyIfDoesntExist + FileFlag_OverwriteReadOnly + FileFlag_SharedFile + FileFlag_RegServer + FileFlag_Touch + FileFlag_UninsNoSharedFilePrompt

; Crash: regsvr32 /u Ir50_32.dll
<File("Files\Ir50_32.dll", "{sys}", SetupFlags_RegServer + FileFlag_UninsNeverUninstall)>
<File("..\LGVID.AX", "{app}", SetupFlags_RegServer)>

<RegAppPaths("shock2.exe")>

<Reg("HKLM\SOFTWARE\Looking Glass Studios")>
<Reg("HKLM\SOFTWARE\Looking Glass Studios\System Shock(tm) 2")>
<Reg("HKLM\SOFTWARE\Looking Glass Studios\System Shock(tm) 2\1.0", "NeverAgain:dword", 0)>

<CustomMessage("en.Menu_SystemShockSeries",		"System Shock Series")>
<CustomMessage("en.Menu_HowToRunSystemShock2",	"How To Run System Shock 2 on Windows 2000-XP")>
<CustomMessage("en.Menu_SystemShockSeriesFAQ",	"System Shock Series FAQ")>

<CustomMessage("en.Menu_TTLG",					"Through the Looking Glass")>
<CustomMessage("en.Menu_TTLGForums",			"Through the Looking Glass Forums")>
<CustomMessage("en.Menu_ThroughTheLookingGlass","Through the Looking Glass")>
<CustomMessage("en.Menu_IrrationalGames",		"System Shock 2 - Irrational Games")>
<CustomMessage("en.Menu_Sshock2com",			"System Shock 2 - Through the Looking Glass")>

<CustomMessage("en.Menu_Upgrades",				"Textures Upgrade")>
<CustomMessage("en.Menu_SHTUP",					"SHTUP - Shock Texture Upgrade Project")>

<CustomMessage("en.Menu_Community",				"Community")>
<CustomMessage("en.Menu_TriOptimum",			"Корпоративная сеть Tri-Optimum")>
<CustomMessage("en.Menu_Sshockcomru",			"System Shock 2 Russian Community")>
<CustomMessage("en.Menu_SystemShock2InfoHub",	"System Shock 2 Information Hub")>

<CustomMessage("en.Menu_PatchReadMe",			"Patch")>

<IconUrlWeb("http://www.ttlg.com/forums/forumdisplay.php?f=64",		"{cm:Menu_SystemShockSeries}")>
<IconUrlWeb("http://www.ttlg.com/forums/showthread.php?t=60930",	"{cm:Menu_SystemShockSeriesFAQ}")>
<IconUrlWeb("http://www.ttlg.com/",									"{cm:Menu_ThroughTheLookingGlass}")>
<IconUrlWeb("http://www.irrationalgames.com/shock2/",				"{cm:Menu_IrrationalGames}")>
<IconUrlWeb("http://www.ttlg.com/forums/showthread.php?t=69958",	"{cm:Menu_HowToRunSystemShock2}")>
<IconUrlWeb("http://shock.fastbb.ru/",								"{cm:Menu_TriOptimum}")>
<IconUrlWeb("http://shtup.home.att.net/",							"{cm:Menu_SHTUP}")>
<IconUrlWeb("http://www.timmymagic.com/sshock2/",					"{cm:Menu_SystemShock2InfoHub}")>
<IconUrlWeb("http://www.sshock2.com/",								"{cm:Menu_Sshock2com}")>
<IconUrlWeb("http://shock.com.ru/",									"{cm:Menu_Sshockcomru}")>

<IconCheats()>
<IconWalkthrough("{app}\Cheats\ss2walk\index.html")>
<IconEggs()>
<IconFaq("{app}\Cheats\SShock2FAQ.doc")>

<IconReadme("{app}\readmep.wri", "{cm:Menu_PatchReadMe}", "{cm:Menu_PatchReadMe}")>

<Dir("{app}\current")>

<UninstallDelete("{app}\current", DeleteFlag_DirIfEmpty)>

<Debug_ViewTranslation>
