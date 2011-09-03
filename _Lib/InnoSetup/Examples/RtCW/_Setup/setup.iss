#include "../../_Common/Config.isi"

#define OutputBaseFilename      "RtCWSetup"

#define AppExe                  "WolfSP.exe"
#define AppMultiplayer          "{app}\WolfMP.exe"

#define AppVersion              "1.41"
#define SetupVersion            AppVersion + ".0.0"

#define AppName             	"Return to Castle Wolfenstein"

#define AppReadmeFile			"{app}\Docs\readme.txt"
#define AppManualFile			"{app}\Docs\Help\Manual\default.htm"
#define AppHelpFile			    "{app}\Docs\Help\index.htm"

#define LicenseFile          	"..\Docs\license.rtf"

#define AppPublisher			"Activision Publishing, Inc."

#define DirectXVersion		    "8.0"

#define AppPublisherURL         "http://www.activision.com/"

#define UrlOfficialSite         "http://www.castlewolfenstein.com/"

#include "../../_Common/Setup.isi"

#include "Files.iss"

<CustomMessage("en.Multiplayer",           "Multiplayer")>
<CustomMessage("en.Menu_Multiplayer",      AppName + " Multiplayer")>

<Reg("HKLM\SOFTWARE\Activision\Return to Castle Wolfenstein", "Version:string", AppVersion)>

<IconCheats()>

<IconRun(name="{group}\{cm:Menu_Multiplayer}", filename=AppMultiplayer, comment="{cm:Menu_Multiplayer}")>

#define Active_Tasks "mp_desktopicon"
    <Task(Active_Tasks, "{cm:CreateDesktopIcon} ({cm:Multiplayer})", "{cm:AdditionalIcons}")>
    <IconRun(name="{commondesktop}\{cm:Menu_Multiplayer}", filename=AppMultiplayer, comment="{cm:Menu_Multiplayer}")>
<Reset_ActiveTasks()>

#ifndef OFF_QUICK_LAUNCH_ICON
    #define Active_Tasks "mp_quicklaunchicon"
        <Task(Active_Tasks, "{cm:CreateQuickLaunchIcon} ({cm:Multiplayer})", "{cm:AdditionalIcons}", "", TaskFlag_Unchecked)>
        <IconRun(name="{userappdata}\Microsoft\Internet Explorer\Quick Launch\{cm:Menu_Multiplayer}", filename=AppMultiplayer, comment="{cm:Menu_Multiplayer}")>
    <Reset_ActiveTasks()>
#endif

<Debug_ViewTranslation()>

