#include "../../_Common/Config.isi"

#define OutputBaseFilename      "TypingOfTheDeadSetup"

#define AppName             	"Typing of the Dead"
#define AppExe                  "Tod_e.exe"

#define AppVersion              "1.0"
#define SetupVersion            AppVersion + ".0.0"

#define AppPublisher			"Sega Enterprises"
#define AppPublisherURL         "http://www.empire.co.uk/"

#define AppReadmeFile			"{app}\Cheats\walkthrough.txt"

#define InfoAfterFile           "Text\InfoAfter.rtf"

#define DirectXVersion          "7.0"

#define ExtraDiskSpaceRequired  410000000

#include "../../_Common/Setup.isi"

#include "Ogg2Mp3.iss"
#include "Files.iss"
#include "Registry.iss"

<FileStub(Ogg2Mp3BatPath, FileFlag_DeleteAfterInstall)>
#define Active_AfterInstall "CreateOgg2Mp3Bat"
    <File("..\oggdec.exe", "{app}", FileFlag_DeleteAfterInstall)>
<Reset_ActiveAfterInstall()>

<IconCheats("cheats.txt")>
<IconWalkthrough("walkthrough.txt")>

<Run(filename=Ogg2Mp3BatPath, flags=RunFlag_WaitUntilTerminated, statusMsg="{cm:StatusMsg_DecodingSounds}")>

<CustomMessage("en.StatusMsg_DecodingSounds", "Decoding sounds. Please wait...")>

<UninstallDelete("{app}\sound", DeleteFlag_FilesAndOrDirs)>

<Debug_ViewTranslation>

