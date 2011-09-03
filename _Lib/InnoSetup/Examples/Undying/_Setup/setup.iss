#include "../../_Common/Config.isi"

#define OutputBaseFilename      "UndyingSetup"

#define AppName              	"Clive Barker's Undying"
#define AppExe                  "System\Undying.exe"

#define AppVersion              "1.0"

#define SetupVersion            AppVersion + ".0.0"

#define AppPublisher            "Electronic Arts"
#define AppPublisherURL         "http://www.ea.com/"
#define AppSupportURL           "http://support.ea.com/"

#define AppReadmeFile           "{app}\readme.txt"

#define DirectXVersion          "8.0"

#include "../../_Common/Setup.isi"

#include "Files.iss"
#include "Registry.iss"

<Dir("{app}\Save")>

<UninstallDelete("{app}\System\*.log")>
<UninstallDelete("{app}\System\*.ini")>

<Debug_ViewTranslation>

