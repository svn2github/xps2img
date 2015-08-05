#define ISM_CleanTranslationOnly

#ifndef UNICODE
    #error Unicode InnoSetup required
#endif

#ifndef BinariesPath
    #define BinariesPath          "..\_bin\Release\"
#endif

#if !Pos("release\", LowerCase(BinariesPath))
    #define DEBUG
#endif

#define ISM_RootDir             AddBackslash(CompilerPath) + "Include/ISM"

#include ISM_RootDir + "/Include/WinVer.isi"

#define MinVersion              WindowsXP

#define OutputBaseFilename      "Xps2ImgSetup"

#define PrivilegesRequired      "none"

#define AppMutex                "Xps2ImgInnoSetupGuard"

#define AppName                 "XPS to Images Converter"

#define AppNamePart             "{app}\xps2img"
#define AppExe                  AppNamePart + "UI.exe"
#define AppChm                  AppNamePart + ".chm"
#define AppExeFileName          ExtractFileName(AppExe)
#define AppSettingsFile         "{code:AppSettingsFilePath}\" + ChangeFileExt(AppExeFileName, "settings")

#define AppVersion              GetFileVersion(BinariesPath + AppExeFileName)

#define AppPublisherURL         "http://xps2img.sf.net"
#define AppUpdatesURL           "http://sourceforge.net/projects/xps2img/files/Releases/"
#define AppSupportURL           "http://sourceforge.net/projects/xps2img/support"

#define ArchitecturesInstallIn64BitMode "x64"

#define OutputDir               "_Output"

#define VersionInfoCopyright    StringChange("Copyright © 2010-%YEAR%, Ivan Ivon", "%YEAR%", GetDateTimeString("yyyy", "", ""))
#define VersionInfoCompany      AppPublisherURL
#define VersionInfoDescription  AppName + " Setup"

#define AppComments             "XPS (XML Paper Specification) document to set of images conversion utility."
#define AppContact              VersionInfoCompany
#define AppPublisher            "Ivan Ivon (" + VersionInfoCompany + ")"
#define AppReadmeFile           "{app}\xps2img.chm"

#define Uninstallable           "IsInstallable"
#define CreateUninstallRegKey   "IsInstallable"

#define AllowNoIcons            "yes"

#define DisableWelcomePage      "no"
#define DisableDirPage          "no"
#define DisableProgramGroupPage "no"

#define SetupIconFile           "Icons/Application.ico"

#define WizardImageFile         "Images/WizardImage.bmp"
#define WizardSmallImageFile    "Images/WizardSmallImage.bmp"

#define X2IFileExtension        "x2i"
#define XPSFileExtension        "xps"
#define X2IFileDescription      AppName + " Settings"

#define FirewallGroup           "i2van"

#define Task_RegisterExtension  "registerextension"
#define Task_AddWFRule          "addfwrule"

#include ISM_RootDir + "/Include/Extra/Code/SingleSetupInstance.isi"
#include ISM_RootDir + "/Include/Extra/Code/RegisterOpenWith.isi"
#include ISM_RootDir + "/Include/Extra/Code/WindowsFirewall.isi"
#include ISM_RootDir + "/Include/Extra/Code/CmdLine.isi"
#include ISM_RootDir + "/Include/Extra/Code/Convert.isi"
#include ISM_RootDir + "/Include/Extra/Code/NETFW.isi"
#include ISM_RootDir + "/Include/Extra/Code/MSXML.isi"

#include "Code/Params.iss"
#include "Code/Code.iss"
#include "Code/Events.iss"

#include ISM_RootDir + "/Include/IncludeAll.isi"

#define Common_RunFlags     RunFlag_NoWait + RunFlag_PostInstall + RunFlag_SkipIfSilent

#define ApplicationFile(name, indexing=false)  Local[0]=BinariesPath + name, FileExists(Local[0]) ? File(source=Local[0], flags=FileFlag_Defaults + (indexing ? '' : FileFlag_NotContentIndexed)) : InstallDelete("{app}\" + name)

#define PortableMarkFile    "xps2imgUI.exe.portable"

#include "Lang/en/Messages.iss"
#include "Lang/uk/Messages.iss"

#include "Code/GenCode.iss"

<Message("BeveledLabel", VersionInfoCompany)>

<ApplicationFile("xps2img.exe", true)>
<ApplicationFile("xps2imgUI.exe", true)>
<ApplicationFile("xps2imgShared.dll")>
<ApplicationFile("xps2imgLib.dll")>
<ApplicationFile("CommandLine.dll")>
<ApplicationFile("Gnu.Getopt.dll")>
<ApplicationFile("Microsoft.WindowsAPICodePack.dll")>
<ApplicationFile("xps2img.exe.config")>
<ApplicationFile("xps2imgUI.exe.config")>
<ApplicationFile("xps2img.chm", true)>

#ifdef DEBUG
    #define DebugFile(f)        File(BinariesPath + f)
    #define DebugLangFile(l)    File(BinariesPath + l + "\*", "{app}\" + l)
    
    <DebugFile("*.pdb")>
    <DebugLangFile("uk")>
#endif

#define Active_Check    "IsPortable"
    <ApplicationFile(PortableMarkFile)>
<Reset_ActiveCheck>

#define Active_Check     "ManageAppSettingsFile('" + AppSettingsFile + "')"
    <FileStub(AppSettingsFile)>
<Reset_ActiveCheck>

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
    <Icon(Const_UninstallGroup + Utils_CmFormat("UninstallProgram", AppName), "{uninstallexe}")>
<Reset_ActiveCheck>

<Icon("{group}\{cm:License}", "{app}\license.rtf")>

<IconRun(AddBackslash("{group}") + AppName, AppExe)>
<IconRun("{group}\{cm:Help}", AppReadmeFile)>

<Run(filename=AppExe, flags=Utils_RemoveFlag(RunFlag_SkipIfSilent, Common_RunFlags), description=Utils_CmFormat("LaunchProgram", AppName))>
<Run(filename=AppChm, flags=Common_RunFlags + RunFlag_ShellExec + RunFlag_Unchecked, description=Utils_CmFormat("ViewHelp", AppName))>

<InstallDelete("{app}\*.url")>
<InstallDelete("{group}", DeleteFlag_FilesAndOrDirs)>

<Debug_ViewTranslation>
