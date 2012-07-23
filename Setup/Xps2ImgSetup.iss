;#define ISM_OnViewCleanTranslation

#define ISM_RootDir             AddBackslash(CompilerPath) + "Include/ISM"

#define OutputBaseFilename      "Xps2ImgSetup"

#define AppMutex                "Xps2ImgInnoSetupGuard"

#define AppName                 "XPS to Images Converter"
#define AppVersion              "3.12.0.0"

#define AppNamePart             "{app}\xps2img"
#define AppExe                  AppNamePart + "UI.exe"
#define AppChm                  AppNamePart + ".chm"

#define UrlOfficialSite         "http://xps2img.sf.net"

#define ArchitecturesInstallIn64BitMode "x64"

#define LicenseFile             "Docs\license.rtf"

#define OutputDir               "_Output"

#define VersionInfoCopyright	"Copyright © 2010-2012, Ivan Ivon"
#define VersionInfoCompany      UrlOfficialSite
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

#define X2IFileDescription      AppName + " Settings"

#include "Utils.iss"
#include "Params.iss"
#include "CodeMessages.iss"
#include "Events.iss"

#define Uninstallable           "IsInstallable"
#define CreateUninstallRegKey   "IsInstallable"

#include ISM_RootDir + "/Include/IncludeAll.isi"

#define Common_RunFlags     RunFlag_NoWait + RunFlag_PostInstall + RunFlag_SkipIfSilent

<Language("en", "compiler:Default.isl")>

<Message("en.BeveledLabel", VersionInfoCompany)>

<CustomMessage("en.Help",               AppName + " Help")>
<CustomMessage("en.License",            AppName + " License")>
<CustomMessage("en.ViewHelp",           "View " + AppName + " Help")>
<CustomMessage("en.Group_Uninstall",    "Uninstall")>
<CustomMessage("en.Menu_WebSite",       "%1 Web Site")>

#define ApplicationFile(f)  File(BinariesPath + f)

<ApplicationFile("xps2img.exe")>
<ApplicationFile("xps2imgUI.exe")>
<ApplicationFile("CommandLine.dll")>
<ApplicationFile("Gnu.Getopt.dll")>
<ApplicationFile("Microsoft.WindowsAPICodePack.dll")>
<ApplicationFile("xps2img.chm")>
#define Active_Check    "IsPortable"
    <ApplicationFile("xps2imgUI.exe.portable")>
<Reset_ActiveCheck>
<File(LicenseFile)>

#define Active_Tasks    "desktopicon"
    <Task(Active_Tasks, "{cm:CreateDesktopIcon}", "{cm:AdditionalIcons}")>
    <IconRun(AddBackslash("{commondesktop}") + AppName, AppExe)>
<Reset_ActiveTasks>

#define AppUrlGeneric(folder) Local[0]=AddBackslash(folder) + Utils_CmFormat("Menu_WebSite", AppName), Url(Local[0], Local[0], UrlOfficialSite)

<AppUrlGeneric("{app}")>

#define Active_Check    "not WizardNoIcons and IsInstallable"
    <AppUrlGeneric("{group}")>
<Reset_ActiveCheck>

#define Active_Check    "IsInstallable"
    <Icon(AddBackslash("{group}\{cm:Group_Uninstall}") + Utils_CmFormat("UninstallProgram", AppName), "{uninstallexe}")>
    <Icon("{group}\{cm:License}", AddBackslash("{app}") + ExtractFileName(LicenseFile))>
    <IconRun(AddBackslash("{group}") + AppName, AppExe)>
    <IconRun("{group}\{cm:Help}", AppReadmeFile)>
<Reset_ActiveCheck>

<Run(filename=AppExe, flags=Utils_RemoveFlag(RunFlag_SkipIfSilent, Common_RunFlags), description=Utils_CmFormat("LaunchProgram", AppName))>
<Run(filename=AppChm, flags=Common_RunFlags + RunFlag_ShellExec + RunFlag_Unchecked, description=Utils_CmFormat("ViewHelp", AppName))>

#define Active_Check    "IsInstallable"
    <Reg("HKCR\.x2i", ":string", AppName, RegFlag_UninsDeleteKey)>
    <Reg("HKCR\" + AppName, ":string", X2IFileDescription, RegFlag_UninsDeleteKey)>
    <Reg("HKCR\" + AppName + "\DefaultIcon", ":string", AppExe+",0")>
    <Reg("HKCR\" + AppName + "\shell\open\command", ":string", Str_Quote(AppExe) + " " + Str_Quote("%1"))>
<Reset_ActiveCheck>

<Debug_ViewTranslation>
