#include "Code.iss"

<RegAppPaths(RegAppName)>

<Reg("HKLM\SOFTWARE\Looking Glass Studios")>
<Reg("HKLM\SOFTWARE\Looking Glass Studios\" + RegAppSettingsRootKey)>
<Reg("HKLM\SOFTWARE\Looking Glass Studios\" + RegAppSettingsRootKey + "\1.0", "NeverAgain:dword", 0)>

#define InstallCfg	'{app}\install.cfg'

#define Active_AfterInstall "SaveInstallCfg('" + InstallCfg + "')"
	<FileStub(InstallCfg)>
<Reset_ActiveAfterInstall()>

#define SetupFlags_RegServer FileFlag_OnlyIfDoesntExist + FileFlag_OverwriteReadOnly + FileFlag_SharedFile + FileFlag_RegServer + FileFlag_Touch + FileFlag_UninsNoSharedFilePrompt

; Crash: regsvr32 /u Ir50_32.dll
<File("Files\Ir50_32.dll", "{sys}", SetupFlags_RegServer + FileFlag_UninsNeverUninstall)>
<File("..\LGVID.AX", "{app}", SetupFlags_RegServer)>

<File("..\tnhScript.osm", "{app}", Common_FileFlags)>
<File("..\script.osm", "{app}", Common_FileFlags)>
<File("..\darkhooks.dlx", "{app}", Common_FileFlags)>

<ShellLink("{app}\lgvid.lnk", "regsvr32.exe", "lgvid.ax", "{app}", Utils_CmFormat("LaunchProgram", "{cm:ShellLink_RegLgvidAX}"))>
<IconRun("{group}\{cm:ShellLink_RegLgvidAX}", "{app}\lgvid.lnk")>

<CustomMessage("en.ShellLink_RegLgvidAX",               "Video Codec Registration")>
<CustomMessage("en.Menu_ThroughTheLookingGlass",        "Through the Looking Glass")>
<CustomMessage("en.Menu_ThiefSeriesFAQ",                "Thief Series FAQ")>
<CustomMessage("en.Menu_ThiefSeriesGeneralDiscussion",  "Thief Series General Discussion")>
<CustomMessage("en.Menu_DarkFate",                      "Thief - the Dark Fate (Russian)")>
<CustomMessage("en.Menu_KeepOfMetalAndGold",            "The Keep of Metal and Gold - Thief missions")>
<CustomMessage("en.Menu_ShadowdarkKeep",                "Shadowdark Keep - Thief Deadly Shadows and Dark Mod missions")>

<IconUrlWeb("http://www.ttlg.com/",                             "{cm:Menu_ThroughTheLookingGlass}")>
<IconUrlWeb("http://www.ttlg.com/forums/forumdisplay.php?f=83", "{cm:Menu_ThiefSeriesGeneralDiscussion}")>
<IconUrlWeb("http://www.ttlg.com/forums/showthread.php?t=75031","{cm:Menu_ThiefSeriesFAQ}")>
<IconUrlWeb("http://darkfate.ru",                               "{cm:Menu_DarkFate}")>
<IconUrlWeb("http://www.keepofmetalandgold.com",                "{cm:Menu_KeepOfMetalAndGold}")>
<IconUrlWeb("http://www.shadowdarkkeep.com/",                   "{cm:Menu_ShadowdarkKeep}")>

<IconWalkthrough()>

<UninstallDelete("{app}\startmis.sav")>
