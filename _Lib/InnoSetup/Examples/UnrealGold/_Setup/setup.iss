#include "../../_Common/Config.isi"

#define OutputBaseFilename    "UnrealGoldSetup"

#define AppName               "Unreal Gold"
#define AppExe                "System\unreal.exe"
#define AppVersion            "226b"

#define SetupVersion          "1.226.0.0"

#define AppPublisher          "Epic MegaGames & Digital Extremes"
#define AppPublisherURL       "http://www.unrealgold.com/"

#define AppReadmeFile         "{app}\Help\readme.txt"
#define AppManualFile         "{app}\Manual\unreal manual.pdf"

#define DirectXVersion        "6.0"

#define UrlOfficialSite       "http://www.unrealgold.com/"

#include "../../_Common/Setup.isi"

#include "Files.iss"
#include "Registry.iss"

<IconCheats("cheats.txt")>
<IconWalkthrough("walkthrough.txt")>
<IconWalkthrough("{app}\Cheats\napali-walkthrough.txt", "{cm:Menu_NaPali}", "{cm:Menu_NaPali}")>

<IconUrlWeb("http://www.unreal.com/", "{cm:Menu_UnrealUniverse}")>

<CustomMessage("en.Menu_NaPali", "Unreal - Return to Na Pali")>
<CustomMessage("en.Menu_UnrealUniverse", "Unreal Universe")>

<UninstallDelete("{app}\System\*.log")>

<Debug_ViewTranslation>

