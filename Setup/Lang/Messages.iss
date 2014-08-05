#define private LicenseFileLocalized    "Docs\" + ActiveLanguage + "\license.rtf"

<Language(ActiveLanguage, MessagesFile, LicenseFileLocalized)>

<LocalizedCustomMessage(ActiveLanguage, "Help",               Help)>
<LocalizedCustomMessage(ActiveLanguage, "License",            License)>
<LocalizedCustomMessage(ActiveLanguage, "ViewHelp",           ViewHelp)>
<LocalizedCustomMessage(ActiveLanguage, "Menu_WebSite",       Menu_WebSite)>

<LocalizedCustomMessage(ActiveLanguage, "Task_RegisterFileAssociations",      Task_RegisterFileAssociations)>
<LocalizedCustomMessage(ActiveLanguage, "Task_AddWindowsFirewallException",   Task_AddWindowsFirewallException)>
<LocalizedCustomMessage(ActiveLanguage, "Task_SystemIntegrationTitle",        Task_SystemIntegrationTitle)>

; .NET check.
<LocalizedCustomMessage(ActiveLanguage, "Msg_DotNetIsMissing", Msg_DotNetIsMissing)>
<LocalizedCustomMessage(ActiveLanguage, "Msg_KeepSettings",    Msg_KeepSettings)>

; Setup mode.
<LocalizedCustomMessage(ActiveLanguage, "Msg_SetupMode",                   Msg_SetupMode)>
<LocalizedCustomMessage(ActiveLanguage, "Msg_SetupModeReadyPage",          Msg_SetupModeReadyPage)>
<LocalizedCustomMessage(ActiveLanguage, "Msg_SetupModeQuestion",           Msg_SetupModeQuestion)>
<LocalizedCustomMessage(ActiveLanguage, "Msg_SetupModeGroupTitle",         Msg_SetupModeGroupTitle)>
<LocalizedCustomMessage(ActiveLanguage, "Msg_SetupModeInstall",            Msg_SetupModeInstall)>
<LocalizedCustomMessage(ActiveLanguage, "Msg_SetupModeInstallUserOnly",    Msg_SetupModeInstallUserOnly)>
<LocalizedCustomMessage(ActiveLanguage, "Msg_SetupModePortable",           Msg_SetupModePortable)>

#define Active_Languages ActiveLanguage
    <File(LicenseFileLocalized)>
<Reset_ActiveLanguages>

<NETFW_CustomMessages(ActiveLanguage)>
