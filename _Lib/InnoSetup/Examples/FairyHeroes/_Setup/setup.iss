#include "../../_Common/Config.isi"

#define USE_LANG_RU

#define OutputBaseFilename	"FairyHeroesSetup"

#define AppName             "{cm:AppName}"

#define AppName_en          "Fairy Heroes"
#define AppName_ru          "Герои Сказок"

#define AppExe              "FairyHeroes.exe"
#define EditorExe           "{app}\Data\maps\map_editor\map_editor.xls"

#define AppVersion          "0.6.0.3"

#define AppPublisher        ""
#define AppPublisherURL     "http://www.is.svitonline.com/skmain"

#define AppReadmeFile       "{app}\Data\readme.txt"

#define AppHelpFile         "{app}\Data\concept.doc"

#define DirectXVersion      "8.0"

#define UrlOfficialSite     "http://www.is.svitonline.com/skmain"

#define AppInfoBeforeFile   "..\Data\readme_en.txt"

#if FileExists(AppInfoBeforeFile)
    #define InfoBeforeFile      AppInfoBeforeFile
#endif

#define AppInfoBeforeFile_ru    "..\Data\readme_ru.txt"

#if FileExists(AppInfoBeforeFile_ru)
    #define InfoBeforeFile_ru   AppInfoBeforeFile_ru
#else
    #define InfoBeforeFile_ru   ""
#endif

#define AppLicenseFile          "..\Data\license_en.txt"

#if FileExists(AppLicenseFile)
    #define LicenseFile         AppLicenseFile
#else
    #define LicenseFile         ""
#endif

#define AppLicenseFile_ru       "..\Data\license_ru.txt"

#if FileExists(AppLicenseFile_ru)
    #define LicenseFile_ru      AppLicenseFile_ru
#else
    #define LicenseFile_ru      ""
#endif

#include "../../_Common/Setup.isi"

#define Common_FileFlags        FileFlag_Touch + FileFlag_IgnoreVersion + FileFlag_OverwriteReadOnly

#include "Files.iss"

<Language("ru", "compiler:Languages\Russian.isl", LicenseFile_ru, InfoBeforeFile_ru)>

<CustomMessage("en.AppName", "Fairy Heroes")>
<CustomMessage("ru.AppName", "Герои Сказок")>

#define LanguageDependentFiles(str lang) \
    File("..\Data\concept_" + lang + ".doc", "{app}\Data", Common_FileFlags, "concept.doc") + \
    File("..\Data\readme_" + lang + ".txt", "{app}\Data", Common_FileFlags, "readme.txt") + \
    File("..\Data\maps\map_editor\readme_" + lang + ".txt", "{app}\Data\maps\map_editor", Common_FileFlags, "readme.txt") + \
    ( \
        (lang == "ru" ? FileExists(AppLicenseFile_ru) : FileExists(AppLicenseFile)) ? \
            File("..\Data\license_" + lang + ".txt", "{app}\Data", Common_FileFlags, "license.txt") : \
            "" \
    )

; Нет английской версии редактора.
<File("..\Data\maps\map_editor\map_editor.xls", "{app}\Data\maps\map_editor", Common_FileFlags)>

#define Active_Languages    "en"
    <LanguageDependentFiles(Active_Languages)>
#define Active_Languages    "ru"
    <LanguageDependentFiles(Active_Languages)>
<Reset_ActiveLanguages()>

<UninstallDelete("{app}\Data\Settings\score")>
<UninstallDelete("{app}\Data\Settings", DeleteFlag_DirIfEmpty)>
<UninstallDelete("{app}\Data", DeleteFlag_DirIfEmpty)>

<UninstallDelete("{app}\applog.txt")>
<UninstallDelete("{app}\errors_log.txt")>

<Debug_ViewTranslation>

