#define protected ActiveLanguage                        "uk"
#define protected MessagesFile                          "compiler:Languages\Ukrainian.isl"
    
#define protected Help                                  AppName + " ������ (����������)"
#define protected License                               AppName + " ˳�����"
#define protected ViewHelp                              "�������� " + AppName + " ������ (����������)"
#define protected Menu_WebSite                          "%1 � ��������"
    
#define protected Task_RegisterFileAssociations         "�&��������� ������ ���������"
#define protected Task_AddWindowsFirewallException      "������ &������� �� Windows Firewall"
#define protected Task_SystemIntegrationTitle           "�������� ����������:"

#define protected Msg_InstallToCurrentDirectory         "���������� � &������� �����"
    
#define protected Msg_DotNetIsMissing                   "�������� ������� ������������ Microsoft.NET 3.5%n%n������ ����������� ���� �����?%n%n������ ��������� ��� ���������� ������������. ������� �����, �� �������������� �������� � ������ ������� �� �����������."
#define protected Msg_KeepSettings                      "������ �������� �������� ������������ ��������?"
    
#define protected Msg_SetupMode                         "���������� ��"
#define protected Msg_SetupModeReadyPage                "���������� ��������:"
#define protected Msg_SetupModeQuestion                 "������ ���������� �������� �������� ��� ����������?"
#define protected Msg_SetupModeGroupTitle               "���������� ��������:"
#define protected Msg_SetupModeInstall                  "&��������"
#define protected Msg_SetupModeInstallUserOnly          "ҳ���� ��� &����. ��� ���������� ������������ �������� �������� � ������� ������������."
#define protected Msg_SetupModePortable                 "&����������"

#include "../Messages.iss"

<NETFW_CustomMessages(ActiveLanguage)>
