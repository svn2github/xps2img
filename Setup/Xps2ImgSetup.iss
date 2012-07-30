#define ISM_CleanTranslationOnly

#define ISM_RootDir             AddBackslash(CompilerPath) + "Include/ISM"

#include ISM_RootDir + "/Include/WinVer.isi"

#define MinVersion              WindowsXP

#define OutputBaseFilename      "Xps2ImgSetup"

#define PrivilegesRequired      "none"

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

#include ISM_RootDir + "/Include/Extra/SingleSetupInstance.iss"

#include "Messages.iss"

#include "Code/Utils.iss"
#include "Code/Params.iss"
#include "Code/Events.iss"

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

#define PortableMarkFile    "xps2imgUI.exe.portable"

<ApplicationFile("xps2img.exe")>
<ApplicationFile("xps2imgUI.exe")>
<ApplicationFile("CommandLine.dll")>
<ApplicationFile("Gnu.Getopt.dll")>
<ApplicationFile("Microsoft.WindowsAPICodePack.dll")>
<ApplicationFile("xps2img.chm")>
#define Active_Check    "IsPortable"
    <ApplicationFile(PortableMarkFile)>
<Reset_ActiveCheck>
<File(LicenseFile)>

<UninstallDelete(AddBackslash("{app}") + PortableMarkFile)>

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

#define RegisterDocumentExtension(str root) \
    Reg(AddBackslash(root) + ".x2i",  ":string", AppName,            RegFlag_UninsDeleteKey) + \
    Reg(AddBackslash(root) + AppName, ":string", X2IFileDescription, RegFlag_UninsDeleteKey) + \
    Reg(AddBackslash(root) + AppName + "\DefaultIcon",        ":string", AppExe + ",0") + \
    Reg(AddBackslash(root) + AppName + "\shell\open\command", ":string", Str_Quote(AppExe) + " " + Str_Quote("%1"))

#define Active_Check    "IsInstallable and IsAdminLoggedOn"
    <RegisterDocumentExtension("HKCR")>
<Reset_ActiveCheck>

#define Active_Check    "IsInstallable and not IsAdminLoggedOn"
    <RegisterDocumentExtension("HKCU\Software\Classes")>
<Reset_ActiveCheck>

<Debug_ViewTranslation>
