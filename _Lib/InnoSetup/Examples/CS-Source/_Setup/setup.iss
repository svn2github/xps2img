#include "../../_Common/Config.isi"

#define USE_LANG_RU

#define OutputBaseFilename	"CounterStrikeSourceSetup"

#define AppName             "Counter-Strike Source"
#define AppExe              "RUN_CSS.exe"

#define AppVersion          "1.0.0.34"

#define AppPublisher        "Valve"
#define AppPublisherURL     "http://www.valvesoftware.com"

#define DirectXVersion      "9"

#define UrlOfficialSite     "http://counter-strike.net/"

#include "../../_Common/Setup.isi"

#include "Files.iss"

<Language("ru", "compiler:Languages\Russian.isl")>

<CustomMessage("en.Menu_GameLanguage", "Game Language")>
<CustomMessage("ru.Menu_GameLanguage", "язык игры")>

<CustomMessage("en.Menu_en", "English")>
<CustomMessage("ru.Menu_en", "јнглийский")>

<CustomMessage("en.Menu_ru", "Russian")>
<CustomMessage("ru.Menu_ru", "–усский")>

<Reg("HKCU\Software\Valve")>
<Reg("HKCU\Software\Valve\Steam")>

#define IconGameLanguage(str lang) \
    File("Files\Registry\" + lang + ".reg", "{app}\setup\registry", Common_FileFlags) + \
    IconRun("{group}\{cm:Menu_GameLanguage}\{cm:Menu_" + lang + "}", "{win}\regedit.exe", "/s ""{app}\setup\registry\" + lang + ".reg""")

<IconGameLanguage("en")>
<IconGameLanguage("ru")>

#define RegGameLanguage(str lang) \
    Reg("HKCU\Software\Valve\Steam", "Language:string", lang)

#define Active_Languages    "en"
    <RegGameLanguage("english")>
#define Active_Languages    "ru"
    <RegGameLanguage("russian")>
<Reset_ActiveLanguages()>

<Debug_ViewTranslation>

