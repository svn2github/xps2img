#define ISM_CleanTranslationOnly

#define ISM_RootDir             AddBackslash(CompilerPath) + "Include/ISM"

#include ISM_RootDir + "/Include/WinVer.isi"

#define MinVersion              WindowsXP

#define OutputBaseFilename      "Xps2ImgSetup"

#define PrivilegesRequired      "none"

#define AppMutex                "Xps2ImgInnoSetupGuard"

#define AppName                 "XPS to Images Converter"
#define AppVersion              "3.22.0.0"

#define AppNamePart             "{app}\xps2img"
#define AppExe                  AppNamePart + "UI.exe"
#define AppChm                  AppNamePart + ".chm"

#define AppPublisherURL         "http://xps2img.sf.net"
#define AppUpdatesURL           "http://sourceforge.net/projects/xps2img/files/Releases/"
#define AppSupportURL           "http://sourceforge.net/projects/xps2img/support"

#define ArchitecturesInstallIn64BitMode "x64"

#define LicenseFile             "Docs\license.rtf"

#define OutputDir               "_Output"

#define VersionInfoCopyright    "Copyright © 2010-2013, Ivan Ivon"
#define VersionInfoCompany      AppPublisherURL
#define VersionInfoDescription  AppName + " Setup"

#define AppComments             "XPS (XML Paper Specification) document to set of images conversion utility."
#define AppContact              VersionInfoCompany
#define AppPublisher            "Ivan Ivon (" + VersionInfoCompany + ")"
#define AppReadmeFile           "{app}\xps2img.chm"

#define AllowNoIcons            "yes"

#define SetupIconFile           "Icons/Application.ico"

#define WizardImageFile         "Images/WizardImage.bmp"
#define WizardSmallImageFile    "Images/WizardSmallImage.bmp"

#define BinariesPath            "..\_bin\Release\"

#define X2IFileExtension        "x2i"
#define XPSFileExtension        "xps"
#define X2IFileDescription      AppName + " Settings"

#define FirewallGroup           "i2van"

#define Task_RegisterExtension  "registerextension"
#define Task_AddWFRule          "addfwrule"

#include ISM_RootDir + "/Include/Extra/Code/SingleSetupInstance.isi"
#include ISM_RootDir + "/Include/Extra/Code/RegisterOpenWith.isi"
#include ISM_RootDir + "/Include/Extra/Code/WindowsFirewall.isi"
#include ISM_RootDir + "/Include/Extra/Code/Utils.isi"

#include "Messages.iss"

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

<CustomMessage("en.Task_RegisterFileAssociations",     "&Register file associations")>
<CustomMessage("en.Task_AddWindowsFirewallException",   "Add &Windows Firewall exception")>
<CustomMessage("en.Task_SystemIntegrationTitle",        "System integration:")>

#define ApplicationFile(f)  File(BinariesPath + f)

#define PortableMarkFile    "xps2imgUI.exe.portable"

<ApplicationFile("xps2img.exe")>
<ApplicationFile("xps2imgUI.exe")>
<ApplicationFile("xps2imgShared.dll")>
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

#define RegisterX2IExtension(int forAll) RegisterDocumentExtension(forAll, X2IFileExtension, X2IFileDescription)

#define Active_Tasks    Task_RegisterExtension
    <Task(Active_Tasks, "{cm:Task_RegisterFileAssociations}", "{cm:Task_SystemIntegrationTitle}")>

    #define Active_Check    "IsAdminLoggedOn"
        <RegisterX2IExtension(true)>
        <RegisterApplication()>
    <Reset_ActiveCheck>

    #define Active_Check    "not IsAdminLoggedOn"
        <RegisterX2IExtension(false)>
    <Reset_ActiveCheck>
<Reset_ActiveTasks>

<Task(Task_AddWFRule, "{cm:Task_AddWindowsFirewallException}", "{cm:Task_SystemIntegrationTitle}")>

#define AppUrlGeneric(folder) Local[0]=AddBackslash(folder) + Utils_CmFormat("Menu_WebSite", AppName), Url(Local[0], Local[0], AppPublisherURL)

<AppUrlGeneric("{app}")>

#define Active_Check    "not WizardNoIcons"
    <AppUrlGeneric("{group}")>
<Reset_ActiveCheck>

#define Active_Check    "IsInstallable"
    <Icon(AddBackslash("{group}\{cm:Group_Uninstall}") + Utils_CmFormat("UninstallProgram", AppName), "{uninstallexe}")>
<Reset_ActiveCheck>

<Icon("{group}\{cm:License}", AddBackslash("{app}") + ExtractFileName(LicenseFile))>
<IconRun(AddBackslash("{group}") + AppName, AppExe)>
<IconRun("{group}\{cm:Help}", AppReadmeFile)>

<Run(filename=AppExe, flags=Utils_RemoveFlag(RunFlag_SkipIfSilent, Common_RunFlags), description=Utils_CmFormat("LaunchProgram", AppName))>
<Run(filename=AppChm, flags=Common_RunFlags + RunFlag_ShellExec + RunFlag_Unchecked, description=Utils_CmFormat("ViewHelp", AppName))>

<Debug_ViewTranslation>
