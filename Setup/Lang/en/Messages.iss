#define private ActiveLanguage          "en"

#define private LicenseFileLocalized    "Docs\" + ActiveLanguage + "\license.rtf"

<Language(ActiveLanguage, "compiler:Default.isl", LicenseFileLocalized)>

<LocalizedCustomMessage(ActiveLanguage, "Help",               AppName + " Help")>
<LocalizedCustomMessage(ActiveLanguage, "License",            AppName + " License")>
<LocalizedCustomMessage(ActiveLanguage, "ViewHelp",           "View " + AppName + " Help")>
<LocalizedCustomMessage(ActiveLanguage, "Menu_WebSite",       "%1 Web Site")>

<LocalizedCustomMessage(ActiveLanguage, "Task_RegisterFileAssociations",      "&Register file associations")>
<LocalizedCustomMessage(ActiveLanguage, "Task_AddWindowsFirewallException",   "Add &Windows Firewall exception")>
<LocalizedCustomMessage(ActiveLanguage, "Task_SystemIntegrationTitle",        "System integration:")>

; .NET check.
<LocalizedCustomMessage(ActiveLanguage, "Msg_DotNetIsMissing", "This application requires Microsoft.NET 3.5 which is not installed.%n%nWould you like to download it now?%n%nPress Cancel to continue installation. Application might not work in this case.")>
<LocalizedCustomMessage(ActiveLanguage, "Msg_KeepSettings",    "Would you like to keep saved application settings?")>

; Setup mode.
<LocalizedCustomMessage(ActiveLanguage, "Msg_SetupMode",                   "Setup Mode")>
<LocalizedCustomMessage(ActiveLanguage, "Msg_SetupModeReadyPage",          "Setup mode:")>
<LocalizedCustomMessage(ActiveLanguage, "Msg_SetupModeQuestion",           "Would you like to install application or setup it as portable?")>
<LocalizedCustomMessage(ActiveLanguage, "Msg_SetupModeGroupTitle",         "Please specify setup mode:")>
<LocalizedCustomMessage(ActiveLanguage, "Msg_SetupModeInstall",            "&Standard installation")>
<LocalizedCustomMessage(ActiveLanguage, "Msg_SetupModeInstallUserOnly",    "In&stall only for me. Run Setup as Administrator for standard installation")>
<LocalizedCustomMessage(ActiveLanguage, "Msg_SetupModePortable",           "P&ortable")>

#define Active_Languages ActiveLanguage
    <File(LicenseFileLocalized)>
<Reset_ActiveLanguages>
