#include "../../../_Common/Config.isi"

#define DIR_LEVEL	"../"

#define OutputBaseFilename      "NOLFSetup"

#define AppName              	"No One Lives Forever"
#define AppVersion              "1.0.0.4"

#define AppExe                  "nolf.exe"

#define AppPublisher			"Fox Interactive, Inc"
#define AppPublisherURL         "http://www.lith.com/games.asp?id=8"
#define AppSupportURL           "http://www.lith.com/popup.asp?id=nolf"

#define AppManualFile           "{app}\Help\Manual.pdf"

#define LicenseFile             "..\license.rtf"

#define AppReadmeFile			"{app}\readme.txt"

#define DirectXVersion          "7.0"

#define UrlOfficialSite         "http://www.noonelivesforever.com/"

#include "../../../_Common/Setup.isi"

#include "Files.iss"
#include "Registry.iss"

#include "../../_Common/Common.iss"

<CustomMessage("en.Menu_Monolith",          "NOLF - MONOLITH")>

<IconUrlWeb("http://www.lith.com/games.asp?id=8",   "{cm:Menu_Monolith}")>

<IconRun(name="{group}\{cm:Menu_StandaloneServer}", filename="{app}\NolfServ.exe", comment="{cm:Menu_StandaloneServer}")>

<UninstallDelete("{app}\FontData.fnt")>
<UninstallDelete("{app}\Nolf.hrf")>
<UninstallDelete("{app}\nolfcmds.txt")>

<Debug_ViewTranslation>

