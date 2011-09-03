#include "../../../_Common/Config.isi"

#define DIR_LEVEL	"../"

#define OutputBaseFilename	"NOLF2Setup"

#define AppName             "No One Lives Forever 2 - A Spy in H.A.R.M.'s Way"
#define AppDirName          "No One Lives Forever 2"
#define AppExe              "nolf2.exe"

#define AppVersion          "1.3.0.0"

#define AppPublisher        "Fox Interactive, Inc"
#define AppPublisherURL     "http://www.lith.com/games.asp?id=6"
#define AppSupportURL       "http://nolf.sierra.com/"

#define LicenseFile         "..\license.rtf"

#define DirectXVersion      "8.1"

#define AppReadmeFile		"{app}\readme.txt"

#define UrlOfficialSite     "http://nolf.sierra.com/"

#include "../../../_Common/Setup.isi"

#include "Files.iss"
#include "Registry.iss"

#include "../../_Common/Common.iss"

// <IconRun(name="{group}\{cm:Menu_StandaloneServer}", filename="{app}\Nolf2Srv.exe", comment="{cm:Menu_StandaloneServer}")>

<UninstallDelete("{app}\launchcmds.txt")>
<UninstallDelete("{app}\BanList.txt")>

<Debug_ViewTranslation>

