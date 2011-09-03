#include "../../_Common/Config.isi"

#define OFF_QUICK_LAUNCH_ICON

#define OutputBaseFilename      "SiNSetup"

#define AppName                 "SiN"
#define AppExe                  "sin.exe"

#define AppVersion              "1.11"
#define SetupVersion            AppVersion + ".0.0"

#define AppPublisher            "Ritual Entertainment"
#define AppPublisherURL         "http://www.ritual.com/"

#define AppReadmeFile           "{app}\readme111.txt"
#define AppManualFile           "{app}\manual.rtf"

#define LicenseFile             "../license.rtf"

#define DirectXVersion          "6.0"

#define UrlOfficialSite         "http://www.ritual.com/sin/index2.html"

#include "../../_Common/Setup.isi"

<Component("sin",       AppName,            "full typical custom",  ComponentFlag_Fixed + ComponentFlag_CheckableAlone)>
<Component("sin\ctf",   "{cm:Menu_SinCTF}", "full",                 ComponentFlag_DontInheritCheck)>
<Component("wos",       "{cm:Menu_WoS}",    "full",                 ComponentFlag_CheckableAlone)>
<Component("wos\ctf",   "{cm:Menu_WoSCTF}", "full",                 ComponentFlag_DontInheritCheck)>

#include "Files.iss"
#include "Registry.iss"

<CustomMessage("en.Menu_CTF",            "CTF")>
<CustomMessage("en.Menu_SinCTF",         AppName + " CTF")>
<CustomMessage("en.Menu_SinRITUALISTIC", "Sin - RITUALISTIC")>

<CustomMessage("en.Menu_WoS",            "Wages of Sin")>
<CustomMessage("en.Menu_WoSCTF",         "Wages of Sin CTF")>
<CustomMessage("en.Task_CreateWoSIcon",  "Create &Wages of Sin desktop icon")>
<CustomMessage("en.Task_CreateCTFIcon",  "Create Sin &CTF desktop icon")>

<IconCheats("cheats.txt")>
<IconWalkthrough("walkthrough.txt")>

<IconUrlWeb("http://www.ritualistic.com/games.php/sin", "{cm:Menu_SinRITUALISTIC}")>

#define Active_Components "sin\ctf"
    #define Active_Tasks "ctficon"
        <Task(Active_Tasks, "{cm:Task_CreateCTFIcon}", "{cm:AdditionalIcons}", Active_Components, TaskFlag_Unchecked)>
    	<IconRun(name="{group}\{cm:Menu_SinCTF}", parameters="+set game ctf", comment="{cm:Menu_SinCTF}")>
    <Reset_ActiveTasks()>
<Reset_ActiveComponents()>

#define Active_Components "wos"
    #define Active_Tasks "wosicon"
        <Task(Active_Tasks, "{cm:Task_CreateWoSIcon}", "{cm:AdditionalIcons}", Active_Components)>
        <IconRun(name="{group}\{cm:Menu_WoS}", parameters="+set game 2015", iconFileName="{app}\2015\Menu_WoS.ico", comment="{cm:Menu_WoS}")>
    <Reset_ActiveTasks()>
    <IconCheats("wos-cheats.txt", "{cm:Menu_WoS}", "{cm:Menu_WoS}")>
    <IconUrlWeb("http://www.ritualistic.com/games.php/sin/wos.php", "{cm:Menu_WoS}")>
    <IconReadme("{app}\2015\Docs\2015_readme.doc", "{group}\{cm:Menu_WoS}\{cm:Menu_Readme}", "{cm:Menu_WoS}")>
    <IconManual("{app}\2015\Docs\manual.doc", "{cm:Menu_WoS}", "{cm:Menu_WoS}")>
<Reset_ActiveComponents()>

#define Active_Components "wos\ctf"
    <IconReadme("{app}\2015\readme.txt", "{group}\{cm:Menu_WoS}\{cm:Menu_FormatReadme,{cm:Menu_CTF}}", "{cm:Menu_WoSCTF}")>
    <IconManual("{app}\2015\WoSCTF Docs\wages of sin ctf documentation.html", "{cm:Menu_WoSCTF}", "{cm:Menu_WoSCTF}")>
<Reset_ActiveComponents()>

<Debug_ViewTranslation()>

