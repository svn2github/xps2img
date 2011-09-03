#include "../../_Common/Config.isi"

#define OutputBaseFilename	"QuakeSetup"

#define AppName             "Quake"
#define AppExe              "QS.exe"
#define AppParameters		"-game ID1"

#define AppVersion          "1.0.9"
#define SetupVersion		AppVersion + ".0"

#define AppPublisher		"Id Software"
#define	AppPublisherURL		"http://idsoftware.com/"

#define AppReadmeFile		"{app}\Help\readme.html"
#define AppManualFile		"{app}\Help\manual.html"

#define LicenseFile			"..\license.rtf"

#define UrlOfficialSite     "http://www.idsoftware.com/games/quake/quake/"

#define DirectXVersion		"3.0"

#include "../../_Common/Setup.isi"

<Type("mp",     "{cm:Type_Custom} - {cm:Group_MissionPacks}")>
<Type("tc",     "{cm:Type_Custom} - {cm:Group_TotalConversions}")>
<Type("qdq",    "{cm:Type_Custom} - {cm:Group_QdQ}")>

<Component("game",              AppName,                      "full typical custom", ComponentFlag_Fixed)>
<Component("add",               "{cm:Group_Addons}",          "full")>
<Component("add\mp",            "{cm:Group_MissionPacks}",    "full mp")>
<Component("add\mp\soa",        "{cm:Group_SOA}",             "full mp")>
<Component("add\mp\doe",        "{cm:Group_DOE}",             "full mp")>
<Component("add\tc",            "{cm:Group_TotalConversions}","full tc")>
<Component("add\tc\malice",     "{cm:Group_Malice}",          "full tc")>
<Component("add\tc\shrak",      "{cm:Group_Shrak}",           "full tc")>
<Component("add\tc\xmen",       "{cm:Group_XMen}",            "full tc")>
<Component("add\qdq",           "{cm:Group_QdQ}",             "full qdq")>
<Component("add\qdq\qd100qr",   "{cm:Group_qd100qr}",         "full qdq")>
<Component("add\qdq\qdqwav",    "{cm:Group_qdqwav}",          "full qdq")>

#include "Files.iss"

<IconRun(name="{group}\{cm:Group_Settings}\{cm:Menu_QS," + AppExe + "}", comment="{cm:Menu_QS}")>
<IconReadme("{app}\Help\glreadme.html", "{group}\{cm:Menu_FormatReadme,{cm:Menu_GLReadme}}", "{cm:Menu_GLReadme}")>
<IconCheats()>

#define Active_Components "add\mp\soa"
	<IconRun(name="{group}\{cm:Group_MissionPacks}\{cm:Group_SOA}", parameters="-game MP1-SoA", comment="{cm:Group_SOA}")>
<Reset_ActiveComponents()>

#define Active_Components "add\mp\doe"
	<IconRun(name="{group}\{cm:Group_MissionPacks}\{cm:Group_DOE}", parameters="-game MP2-DoE", comment="{cm:Group_DOE}")>
<Reset_ActiveComponents()>

#define Active_Components "add\qdq\qd100qr"
	<IconRun(name="{group}\{cm:Group_QdQ}\{cm:Group_qd100qr}\{cm:Group_qd100qr}", parameters="-game QdQ/qd100Qr", comment="{cm:Group_qd100qr}")>
    <IconReadme("{app}\QdQ\qd100Qr\qd100qr.html", "{group}\{cm:Group_QdQ}\{cm:Group_qd100qr}\{cm:Menu_Readme}", "{cm:Group_qd100qr}")>
<Reset_ActiveComponents()>

#define Active_Components "add\qdq\qdqwav"
	<IconRun(name="{group}\{cm:Group_QdQ}\{cm:Group_qdqwav}\{cm:Group_qdqwav}", parameters="-game QdQ/qdqwav", comment="{cm:Group_qdqwav}")>
    <IconReadme("{app}\QdQ\qdqwav\qdqwav.html", "{group}\{cm:Group_QdQ}\{cm:Group_qdqwav}\{cm:Menu_Readme}", "{cm:Group_qdqwav}")>
    <Icon(name="{group}\{cm:Group_QdQ}\{cm:Group_qdqwav}\{cm:Menu_Routes}", filename="{app}\QdQ\qdqwav\routes.html", comment=Utils_CmFormat("View_FormatView", "{cm:Group_qdqwav} {cm:Menu_Routes}"))>
<Reset_ActiveComponents()>

#define Active_Components "add\tc\malice"
	<IconRun(name="{group}\{cm:Group_TotalConversions}\{cm:Group_Malice}\{cm:Group_Malice}", parameters="-game Malice", comment="{cm:Group_Malice}")>
	<IconReadme("{app}\Malice\readme.html", "{group}\{cm:Group_TotalConversions}\{cm:Group_Malice}\{cm:Menu_Readme}", "{cm:Group_Malice}")>
    <IconHints("{app}\Malice\hint.html", "{group}\{cm:Group_TotalConversions}\{cm:Group_Malice}\{cm:Group_Cheats}\{cm:Menu_FormatHints, {cm:Group_Malice}}", "{cm:Group_Malice}")>
    <IconManual("{app}\Malice\malice.html", "{group}\{cm:Group_TotalConversions}\{cm:Group_Malice}\{cm:Group_Help}\{cm:Menu_FormatManual, {cm:Group_Malice}}", "{cm:Group_Malice}")>
    <IconLicense("{app}\Malice\mlicense.html", "{group}\{cm:Group_TotalConversions}\{cm:Group_Malice}\{cm:Menu_License}", "{cm:Group_Malice}")>
<Reset_ActiveComponents()>

#define Active_Components "add\tc\shrak"
	<IconRun(name="{group}\{cm:Group_TotalConversions}\{cm:Group_Shrak}\{cm:Group_Shrak}", parameters="-game Shrak", comment="{cm:Group_Shrak}")>
    <IconReadme("{app}\Shrak\readme.html", "{group}\{cm:Group_TotalConversions}\{cm:Group_Shrak}\{cm:Menu_Readme}", "{cm:Group_Shrak}")>
<Reset_ActiveComponents()>

#define Active_Components "add\tc\xmen"
    <IconRun(name="{group}\{cm:Group_TotalConversions}\{cm:Group_XMen}\{cm:Group_XMen}", parameters="-game XMen", comment="{cm:Group_XMen}")>
    <IconRun(name="{group}\{cm:Group_TotalConversions}\{cm:Group_XMen}\{cm:Group_XMen} {cm:Menu_Launcher}", filename="{app}\XMen\XMEN.EXE", comment="{cm:Group_XMen} {cm:Menu_Launcher}")>
    <IconWalkthrough("{app}\XMen\walkthrough.html", "{group}\{cm:Group_TotalConversions}\{cm:Group_XMen}\{cm:Group_Cheats}\{cm:Menu_FormatWalkthrough,{cm:Group_XMen}}", "{cm:Group_XMen}")>
    <IconLicense("{app}\XMen\llegal.html", "{group}\{cm:Group_TotalConversions}\{cm:Group_XMen}\{cm:Menu_License}", "{cm:Group_XMen}")>
    <IconRun(name="{group}\{cm:Group_TotalConversions}\{cm:Group_XMen}\{cm:Menu_Comics}", filename="{app}\XMen\RAVCOMIC.EXE", comment="{cm:Group_XMen} {cm:Menu_Comics}")>
    <IconReadme("{app}\XMen\readme.html", "{group}\{cm:Group_TotalConversions}\{cm:Group_XMen}\{cm:Menu_Readme}", "{cm:Group_XMen}")>
<Reset_ActiveComponents()>

<CustomMessage("en.Menu_QS",				"Quake Selector")>
<CustomMessage("en.Menu_GLReadme",			"Open GL")>
<CustomMessage("en.Menu_Launcher",			"Launcher")>
<CustomMessage("en.Menu_Comics",			"Comics")>
<CustomMessage("en.Menu_Routes",			"History of The Routes")>

<CustomMessage("en.Group_Settings",			"Settings")>
<CustomMessage("en.Group_QdQ",				"Quake done Quick")>
<CustomMessage("en.Group_MissionPacks",		"Mission Packs")>
<CustomMessage("en.Group_SOA",				"Mission Pack 1 - Scourge of Armagon")>
<CustomMessage("en.Group_DOE",				"Mission Pack 2 - Dissolution of Eternity")>
<CustomMessage("en.Group_TotalConversions",	"Total Conversions")>
<CustomMessage("en.Group_Addons",			"Addons")>
<CustomMessage("en.Group_qd100qr",			"Quake done 100% Quicker")>
<CustomMessage("en.Group_qdqwav",			"Quake done Quick with a Vengeance")>
<CustomMessage("en.Group_XMen",				"X-Men")>
<CustomMessage("en.Group_Shrak",			"Shrak")>
<CustomMessage("en.Group_Malice",			"Malice")>

<UninstallDelete("{app}\ID1\glquake",           DeleteFlag_FilesAndOrDirs)>
<UninstallDelete("{app}\MP1-SoA\glquake",       DeleteFlag_FilesAndOrDirs)>
<UninstallDelete("{app}\MP2-DoE\glquake",       DeleteFlag_FilesAndOrDirs)>
<UninstallDelete("{app}\QDQ\qd100Qr\glquake",   DeleteFlag_FilesAndOrDirs)>
<UninstallDelete("{app}\QDQ\qdqwav\glquake",    DeleteFlag_FilesAndOrDirs)>
<UninstallDelete("{app}\SHRAK\glquake",         DeleteFlag_FilesAndOrDirs)>
<UninstallDelete("{app}\Malice\glquake",        DeleteFlag_FilesAndOrDirs)>
<UninstallDelete("{app}\XMen\glquake",          DeleteFlag_FilesAndOrDirs)>
<UninstallDelete("{app}\qs.ini",                DeleteFlag_FilesAndOrDirs)>

<Debug_ViewTranslation()>

