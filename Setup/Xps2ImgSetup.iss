;#define ISM_OnViewCleanTranslation

#define ISM_RootDir		        AddBackslash(CompilerPath) + "Include/ISM"

#define OutputBaseFilename      "Xps2ImgSetup"

#define AppName                 "XPS to Images Converter"
#define AppVersion              "2.1.0.0"

#define AppExe                  "{app}\xps2imgUI.exe"

#define UrlOfficialSite         "http://xps2img.sf.net"

#define ArchitecturesInstallIn64BitMode "x64"

#define LicenseFile             "Docs\license.rtf"

#define OutputDir		        "_Output"

#define VersionInfoCopyright	"Copyright © 2010-2011, Ivan Ivon"
#define VersionInfoCompany      "ivan.ivon@gmail.com"
#define VersionInfoDescription  AppName + " Setup"

#define AppComments             "XPS (XML Paper Specification) document to set of images conversion utility."
#define AppContact              VersionInfoCompany
#define AppPublisher            "Ivan Ivon (" + VersionInfoCompany + ")"
#define AppPublisherURL         UrlOfficialSite
#define AppReadmeFile           "{app}\xps2img.chm"

#define AllowNoIcons            "yes"

#define SetupIconFile           "Icons/Application.ico"

#define WizardImageFile         "Images/WizardImage.bmp"
#define WizardSmallImageFile    "Images/WizardSmallImage.bmp"

#define BinariesPath            "..\_bin\Release\"

#include "Code.iss"

#include ISM_RootDir + "/Include/IncludeAll.isi"

#define Common_RunFlags     RunFlag_NoWait + RunFlag_PostInstall + RunFlag_SkipIfSilent + RunFlag_Unchecked

<Language("en", "compiler:Default.isl")>

<CustomMessage("en.Help",               AppName + " Help")>
<CustomMessage("en.License",            AppName + " License")>
<CustomMessage("en.Group_Uninstall",    "Uninstall")>
<CustomMessage("en.Menu_WebSite",       "%1 Web Site")>

<File(BinariesPath + "xps2img.exe")>
<File(BinariesPath + "xps2imgUI.exe")>
<File(BinariesPath + "CommandLine.dll")>
<File(BinariesPath + "Gnu.Getopt.dll")>
<File(BinariesPath + "xps2img.chm")>
<File(LicenseFile)>

#define Active_Tasks    "desktopicon"
    <Task(Active_Tasks, "{cm:CreateDesktopIcon}", "{cm:AdditionalIcons}")>
    <IconRun(AddBackslash("{commondesktop}") + AppName, AppExe)>
<Reset_ActiveTasks>

#define AppUrlGeneric(folder) Local[0]=AddBackslash(folder) + Utils_CmFormat("Menu_WebSite", AppName), Url(Local[0], Local[0], UrlOfficialSite)

<AppUrlGeneric("{app}")>
#define Active_Check "not WizardNoIcons"
    <AppUrlGeneric("{group}")>
#undef Active_Check 

<Icon(AddBackslash("{group}\{cm:Group_Uninstall}") + Utils_CmFormat("UninstallProgram", AppName), "{uninstallexe}")>
<Icon("{group}\{cm:License}", AddBackslash("{app}") + ExtractFileName(LicenseFile))>
<IconRun(AddBackslash("{group}") + AppName, AppExe)>
<IconRun("{group}\{cm:Help}", AppReadmeFile)>

<Run(filename=AppExe, flags=Common_RunFlags, description=Utils_CmFormat("LaunchProgram", AppName))>

<Debug_ViewTranslation>
