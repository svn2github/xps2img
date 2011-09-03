#include "../../_Common/Config.isi"

#define OutputBaseFilename      "HoMM2GoldSetup"

#define AppName                 "Heroes of Might and Magic II Gold"
#ifndef ISM_OnFileStub
    #define AppVersion          GetFileProductVersion('..\' + AppExe)
#else
    #define AppVersion          "1.2"
#endif
#define SetupVersion            AppVersion + ".0.0"

#define AppExe                  "HEROES2W.EXE"
#define EditorExe               "{app}\EDITOR2W.EXE"

#define AppPublisher			"3DO Company"

#define LicenseFile          	"..\license.rtf"

#define AppReadmeFile			"{app}\readme.txt"
#define AppHelpFile             "{app}\HELP\HEROES2.hlp"
#define AppManualFile           "{app}\H2manual.pdf"

#include "../../_Common/Setup.isi"

#include "Files.iss"
#include "Registry.iss"

<IconCheats()>

<Debug_ViewTranslation>

