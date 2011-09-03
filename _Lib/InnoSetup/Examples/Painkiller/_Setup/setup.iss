#include "../../_Common/Config.isi"

#define OutputBaseFilename      "PainkillerFullSetup"

#define DiskSpanning            "yes"
#define ISM_UseDiskSliceSize700M

#define AppName              	"Painkiller"
#define AppVersion              "1.64.0.0"

#define AppBinFolder            "bin"
#define AppExe                  AppBinFolder + "\painkiller.exe"

#define AppPublisher			"DreamCatcher Interactive"
#define AppPublisherURL         "http://www.dreamcatchergames.com/"
#define AppSupportURL           "http://www.peoplecanfly.com/"

#define AppReadmeFile           "{app}\Docs\readme.txt"

#define LicenseFile             "..\Docs\license.rtf"

#define DirectXVersion          "9.0"

#define UrlOfficialSite         "http://www.painkillergame.com"

#include "../../_Common/Setup.isi"

#include "Files.iss"
#include "Registry.iss"

<CustomMessage("en.Menu_PBOOH",             "Painkiller - Battle Out of Hell")>
<CustomMessage("en.Menu_PainkillerHell",    "Complete Painkiller Walkthroughs")>
<CustomMessage("en.Menu_DedicatedServer",   AppName + " Dedicated Server")>
<CustomMessage("en.Menu_PBOOHSecretsMore",  "Painkiller - Battle Out of Hell - Secrets and More")>
<CustomMessage("en.Menu_PBOOHTaro",         "Painkiller and Battle Out of Hell Black Tarot Card Reference")>
<CustomMessage("en.Menu_PeopleCanFly",      "People Can Fly")>
<CustomMessage("en.Menu_PKEuro",            "Popular Painkiller and Painkiller - Battle Out of Hell")>

<IconUrlWeb("http://www.peoplecanfly.com",      "{cm:Menu_PeopleCanFly}")>
<IconUrlWeb("http://www.pkeuro.com",            "{cm:Menu_PKEuro}")>
<IconUrlWeb("http://www.painkillerhell.com",    "{cm:Menu_PainkillerHell}")>
<IconUrlWeb("http://www.kalme.de/painkiller",   "{cm:Menu_PBOOHSecretsMore}")>
<IconUrlWeb("http://www.kalme.de/painkiller/blacktarots.html", "{cm:Menu_PBOOHTaro}")>

<IconCheats()>
<IconWalkthrough()>
<IconWalkthrough("{app}\Cheats\BOOHWalkthrough.html", "{cm:Menu_PBOOH}", "{cm:Menu_PBOOH}")>

<IconManual("{app}\Docs\BOOHManual.pdf", "{cm:Menu_PBOOH}", "{cm:Menu_PBOOH}")>

<IconRun("{group}\{cm:Menu_DedicatedServer}", "{app}\" + AppExe, "-dedicated", "", "{cm:Menu_DedicatedServer}")>

<Dir("{app}\SaveGames")>
<Dir("{app}\Data\Screenshots")>

<UninstallDelete("{app}\Data", DeleteFlag_DirIfEmpty)>

<Debug_ViewTranslation>

