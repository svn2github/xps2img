#include "../../_Common/Config.isi"

#define OutputBaseFilename      "FAKK2Setup"

#define AppName             	"Heavy Metal - F.A.K.K.2"
#define AppVersion              "1.02"
#define SetupVersion            AppVersion + ".0.0"

#define AppExe                  "fakk2.exe"

#define AppPublisher			"Ritual Entertainment"
#define AppPublisherURL         "http://www.ritual.com/"

#define AppReadmeFile			"{app}\readme.txt"

#define LicenseFile          	"..\license.rtf"

#define DirectXVersion          "7.0"

#define UrlOfficialSite         "http://www.ritualistic.com/games.php/fakk2"

#include "../../_Common/Setup.isi"

#include "Files.iss"
#include "Registry.iss"

<CustomMessage("en.Ritual",     "Ritual Entertainment")>
<CustomMessage("en.GOD",        "Gathering of Developers")>

<IconUrlWeb("http://www.ritual.com/", "{cm:Ritual}")>
<IconUrlWeb("http://www.godgames.com/", "{cm:GOD}")>

<IconCheats()>
<IconWalkthrough()>
<IconEggs()>
<IconFaq()>
<IconHints()>

<Debug_ViewTranslation>

