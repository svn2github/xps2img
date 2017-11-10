﻿; See Xps2ImgUI\Settings\Preferences.Localizations.cs for ActiveLanguageName

#define protected ActiveLanguage                        "uk"
#define protected ActiveLanguageName                    "Ukrainian"
#define protected MessagesFile                          "compiler:Languages\Ukrainian.isl"
    
#define protected Help                                  AppName + " Довідка (Англійською)"
#define protected License                               AppName + " Ліцензія (Англійською)"
#define protected ViewHelp                              "Показати " + AppName + " довідку (Англійською)"
#define protected Menu_WebSite                          "%1 в Інтернеті"
    
#define protected Task_RegisterFileAssociations         "Р&еєструвати файлові асоціації"
#define protected Task_AddWindowsFirewallException      "Додати &виняток до Windows Firewall"
#define protected Task_SystemIntegrationTitle           "Системна інтеграція:"

#define protected Msg_InstallToCurrentDirectory         "Встановити у &поточну папку"
    
#define protected Msg_DotNetIsMissing                   "Програма потребує встановлення Microsoft.NET 3.5%n%nЧи ви бажаєте завантажити його зараз?%n%nНатисніть «Скасувати» для подальшого встановлення. Зверніть увагу, що роботоздатність програми у такому випадку не гарантується."
#define protected Msg_KeepSettings                      "Чи ви бажаєте залишити збережені налаштування програми?"
    
#define protected Msg_SetupMode                         "Встановити як"
#define protected Msg_SetupModeReadyPage                "Встановити програму:"
#define protected Msg_SetupModeQuestion                 "Як ви бажаєте встановити програму – звичайно або портативно?"
#define protected Msg_SetupModeGroupTitle               "Встановити програму:"
#define protected Msg_SetupModeInstall                  "&Звичайно"
#define protected Msg_SetupModeInstallUserOnly          "Тільки для &мене. Для звичайного встановлення запустіть програму з правами адміністратора."
#define protected Msg_SetupModePortable                 "&Портативно"

#include "../Messages.iss"

<NETFW_CustomMessages(ActiveLanguage)>
