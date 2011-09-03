#include "../../_Common/Config.isi"

#define OutputBaseFilename	"BloodRayneSetup"

#define AppName             "BloodRayne"
#define AppExe              "Rayne.exe"

#ifdef ISM_OnFileStub
    #define AppVersion      "1.0.1.0"
#else
    #define AppVersion      GetFileProductVersion('..\' + AppExe)
#endif

#define AppPublisher        "Majesco Entertainment"
#define AppPublisherURL     "http://www.majescogames.com/"

#define AppReadmeFile       "{app}\readme.txt"

#define DiskSpanning        "yes"
#define ISM_UseDiskSliceSize700M

#define DirectXVersion      "8.1"

#define UrlOfficialSite     "http://bloodrayne.com"

#include "../../_Common/Setup.isi"

#include "Files.iss"
#include "Registry.iss"
#include "Saves.iss"

<Component("game",          AppName,                        "full typical custom", ComponentFlag_Fixed)>
<Component("cheats",        "{cm:Group_Cheats}",            "full")>
<Component("cheats\saves",  "{cm:Component_SavedGames}",    "full")>

<CustomMessage("en.Menu_RussianBloodRayneCommunity", "Russian BloodRayne Community")>

<IconUrlWeb("http://bloodrayne2.ru", "{cm:Menu_RussianBloodRayneCommunity}")>

<IconCheats()>
<IconWalkthrough()>
<IconWalkthroughAlt()>

<UninstallDelete("{app}\stderr.txt")>

<Debug_ViewTranslation>

