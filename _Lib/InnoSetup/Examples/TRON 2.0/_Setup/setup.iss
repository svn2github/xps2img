#include "../../_Common/Config.isi"

#define OutputBaseFilename      "TRON2Setup"

#define AppName              	"TRON 2.0"
#define AppExe                  "tron.exe"

#define AppVersion              "1.042"
#define SetupVersion            "1.0.4.2"

#define LicenseFile             "../license.rtf"

#define AppPublisher			"Buena Vista Interactive"
#define AppPublisherURL         "http://buenavistagames.go.com/product/tronPC.html"
#define AppSupportURL           "http://www.lith.com/games.asp?id=4"

#define AppReadmeFile			"{app}\readme.txt"
#define AppManualFile           "{app}\Tron.chm"

#define DirectXVersion          "9.0"

#define UrlOfficialSite         "http://buenavistagames.go.com/product/tronPC.html"

#include "../../_Common/Setup.isi"

#include "Files.iss"
#include "Registry.iss"

<CustomMessage("en.Menu_FanSite",           AppName + " Fan Site")>
<CustomMessage("en.Menu_News",              AppName + " News")>
<CustomMessage("en.Menu_Wiki",              AppName + " at Wikipedia")>
<CustomMessage("en.Menu_FAQ",               AppName + " Unofficial FAQ v1.0")>
<CustomMessage("en.Menu_Subroutines",       AppName + " Subroutine Library")>
<CustomMessage("en.Menu_MPExpansionPatch",  AppName + " Multiplayer Expansion Patch")>
<CustomMessage("en.Menu_DedicatedServer",   "Dedicated Server")>

<IconUrlWeb("http://www.tron-sector.com/",              "{cm:Menu_FanSite}")>
<IconUrlWeb("http://tronfaq.blogspot.com/",             "{cm:Menu_News}")>
<IconUrlWeb("http://www.ldso.net/tronfaq/",             "{cm:Menu_FAQ}")>
<IconUrlWeb("http://en.wikipedia.org/wiki/Tron_2.0",    "{cm:Menu_Wiki}")>

<IconRun(name="{group}\{cm:Menu_DedicatedServer}", filename="{app}\TRONSrv.exe", comment="{cm:Menu_DedicatedServer}")>

<IconCheats()>
<IconWalkthrough()>

<IconReadme("{app}\Tron Update 2 and Dedicated Server Readme.txt", "{group}\{cm:Menu_FormatReadme,{cm:Menu_DedicatedServer}}", "{cm:Menu_DedicatedServer}")>
<IconManual("{app}\Multiplayer_Expansion_Patch_Help.chm", "{cm:Menu_MPExpansionPatch}", "{cm:Menu_MPExpansionPatch}")>

<Icon(name="{group}\{cm:Group_Cheats}\{cm:Menu_Subroutines}", filename="{app}\Cheats\Subroutines.html", comment="{cm:View_FormatView,{cm:Menu_Subroutines}}")>

<Dir("{app}\Save")>

<UninstallDelete("{app}\launchcmds.txt")>
<UninstallDelete("{app}\BanList.txt")>
<UninstallDelete("{app}\bl.dat")>

<Debug_ViewTranslation()>

