#include "../../_Common/Config.isi"

#define OutputBaseFilename      "DukeNukemManhattanProjectSetup"

#define AppName                 "Duke Nukem - Manhattan Project"
#define AppVersion              "1.0.1.0"

#define AppExe                  "DukeNukemMP.exe"

#define AppPublisher			"Sunstorm Interactive"
#define AppPublisherURL         "http://www.sunstorm.net/"

#define AppReadmeFile			"{app}\duke\readme.txt"

#define LicenseFile             "..\license.rtf"

#define DirectXVersion          "8.0"

#include "../../_Common/Setup.isi"

#include "Files.iss"

<CustomMessage("en.AppSettings", AppName + " Quick Settings")>

<IconCheats("cheats.txt")>
<IconWalkthrough("walkthrough.txt")>

<IconRun("{group}\{cm:AppSettings}", "{app}\" + AppExe, "show", "{app}", "{cm:AppSettings}")>

<UninstallDelete("{app}\prism3d.log")>

<Debug_ViewTranslation>

